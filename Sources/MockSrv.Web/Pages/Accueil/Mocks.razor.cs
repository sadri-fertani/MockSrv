using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MockSrv.Web.Modals;
using MockSrv.Web.Models;
using Radzen;
using Radzen.Blazor;

namespace MockSrv.Web.Pages.Accueil;

public partial class Mocks
{
    private string ApiTarget => Configuration.GetValue<string>("Api:IHM");

    private IEnumerable<MockRequestResponseModel> RequestResponses;
    private RadzenDataGrid<MockRequestResponseModel> MyGrid;

    private bool IsLoading = false;
    private bool IsAuthenticated = false;

    /// <summary>
    /// Stores the set of selected column property names.
    /// When null - all columns are selected.
    /// </summary>
    private HashSet<string> selectedColumns;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        IsAuthenticated = SignInManager.IsSignedIn(HttpContextAccessor.HttpContext.User);
    }

    async Task LoadData(LoadDataArgs args)
    {
        IsLoading = true;

        await Task.Yield();

        var listRequestResponse = await MockServerApi.GetAllAsync(CancellationToken.None);

        RequestResponses = await Task.FromResult
        (
            Mapper.Map<List<MockRequestResponseModel>>(listRequestResponse)
        );

        IsLoading = false;
    }

    void OnRender(DataGridRenderEventArgs<MockRequestResponseModel> args)
    {
        if (args.FirstRender)
        {
            args.Grid.Groups.Add(new GroupDescriptor() { Title = localizer["ApiName"], Property = "ApiName", SortOrder = SortOrder.Descending });

            StateHasChanged();
        }
    }

    #region Columns picker
    /// <summary>
    /// Returns a set of key-value pairs to splat as column attributes.
    /// </summary>
    IEnumerable<KeyValuePair<string, object>> GetAttributes(string columnName)
    {
        var isSelected = selectedColumns?.Contains(columnName) ?? true;
        return [
            new ("Property", columnName),
            new ("Visible", isSelected),
        ];
    }
    /// <summary>
    /// Displays the column picker in a context menu.
    /// </summary>
    void ShowContextMenuColumnPicker(MouseEventArgs args)
    {
        ContextMenuService.Open(args, ds =>
            (builder) =>
            {
                builder.OpenComponent<RadzenListBox<object>>(0);

                builder.AddAttribute(1, "Style", "height:200px;background-color:grey;font-weight: bold;color:black;");
                builder.AddAttribute(2, "SelectAllText", "All");
                builder.AddAttribute(3, "AllowSelectAll", true);
                builder.AddAttribute(4, "AllowFiltering", true);
                builder.AddAttribute(5, "FilterCaseSensitivity", FilterCaseSensitivity.CaseInsensitive);
                builder.AddAttribute(6, "Multiple", true);
                builder.AddAttribute(7, "Placeholder", "All");
                builder.AddAttribute(8, "Data", ExtractPickableColumns());
                builder.AddAttribute(9, "TextProperty", "Title");
                builder.AddAttribute(10, "Value", CalculateValuesPickableColumns());
                builder.AddAttribute(11, "Change", EventCallback.Factory.Create<object>(this, (obj) => RadzenListBoxSelectionChange(obj)));

                builder.CloseComponent();
            }
        );
    }

    private IEnumerable<RadzenDataGridColumn<MockRequestResponseModel>> ExtractPickableColumns()
    {
        foreach (var columnN1 in MyGrid.ColumnsCollection.Where(cc => cc.ColumnsCollection.Count > 0))
        {
            foreach (var columnN2 in columnN1.ColumnsCollection.Where(c => c.Pickable))
                yield return columnN2;
        }
    }

    private IEnumerable<RadzenDataGridColumn<MockRequestResponseModel>> CalculateValuesPickableColumns()
    {
        foreach (var columnN1 in MyGrid.ColumnsCollection.Where(cc => cc.ColumnsCollection.Count > 0))
        {
            foreach (var columnN2 in columnN1.ColumnsCollection.Where(c => c.Pickable && c.GetVisible()))
                yield return columnN2;
        }
    }

    /// <summary>
    /// Reacts to the changes of selected columns in the context menu.
    /// </summary>
    void RadzenListBoxSelectionChange(object args)
    {
        this.selectedColumns = ((IEnumerable<object>)args)
            .Cast<RadzenDataGridColumn<MockRequestResponseModel>>()
            .Select(c => c.Property)
            .ToHashSet();
    }
    #endregion

    private async Task CopyToClipBoard(string apiTarget, string requestPath, string requestQueryString)
    {
        await JSRuntime.InvokeVoidAsync("eval", $"navigator.clipboard.writeText('{apiTarget}{requestPath}{requestQueryString}')");
        NotificationService.Notify(NotificationSeverity.Info, "", localizer["CopiedClipboard"], 4000);
    }

    void ShowNotification(NotificationMessage message)
    {
        NotificationService.Notify(message);
    }

    async Task DeleteRow(MockRequestResponseModel row)
    {
        var result = await DialogService.Confirm
            (
                localizer["MsgDialogDelete"],
                $"{localizer["TitleDialogDeleteMock"]} : {row.RequestPath}",
                new ConfirmOptions() { OkButtonText = localizer["Yes"], CancelButtonText = localizer["No"] }
            );

        if (result.Value)
        {
            try
            {
                await MockServerApi.DeleteAsync(row.Id, CancellationToken.None);

                // Notification
                ShowNotification(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = localizer["Success"],
                        Detail = localizer["MsgSuccessDeleteMock"],
                        Duration = 4000
                    }
                );

                await MyGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error {DeleteRow}", nameof(DeleteRow));

                // Notification
                ShowNotification(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = localizer["Error"],
                        Detail = localizer["MsgInternalError"],
                        Duration = 4000
                    }
                );
            }
        }
    }

    async Task EditRow(MockRequestResponseModel row)
    {
        var result = await DialogService.OpenAsync<EditMock>(
            row is null ? localizer["TitleModalNewMock"] : localizer["TitleModalUpdateMock"],
            new Dictionary<string, object>()
            {
                {
                    "CurrentMock", row ?? new MockRequestResponseModel { }
                },
                {
                    "ActionEdition", row is null ? TypeOperationEdition.Creation : TypeOperationEdition.Modification
                }
            },
            new DialogOptions() { }
            );

        if (result is RetourEditionModel retourEdition)
        {
            if (retourEdition.Success)
            {
                // Notification
                ShowNotification(
                    new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = localizer["Success"],
                        Detail = retourEdition.TypeOperation == TypeOperationEdition.Creation ? localizer["MsgSuccessAddMock"] : localizer["MsgSuccessUpdateMock"],
                        Duration = 4000
                    }
                );

                await MyGrid.Reload();
            }
            else
            {
                if (!string.IsNullOrEmpty(retourEdition.Message) || retourEdition.Exception)
                {
                    Logger.LogError(retourEdition.Exception ? retourEdition.Message : "Error {EditRow}", nameof(EditRow));

                    // Notification
                    ShowNotification(
                        new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = localizer["Error"],
                            Detail = retourEdition.Message,
                            Duration = 4000
                        }
                    );
                }
            }
        }
    }
}
