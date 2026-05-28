using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MockSrv.Application.DTOs;
using MockSrv.Domain.Extensions;
using MockSrv.Domain.Globals;
using MockSrv.Web.Models;
using MockSrv.Web.Services;

namespace MockSrv.Web.Modals;

public partial class EditMock
{
    [Inject]
    public IMockServerApi MockServerApi { get; set; }

    [Inject]
    public IMapper Mapper { get; set; }

    [Inject]
    public ILogger<EditMock> Logger { get; set; }

    [Parameter]
    public MockRequestResponseModel CurrentMock { get; set; }

    [Parameter]
    public TypeOperationEdition ActionEdition { get; set; }

    private RetourEditionModel Resultat { get; set; }

    private EditContext ContextEdit;
    private bool IsInvalid { get; set; }

    private MockRequestResponseModel Model { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model = CurrentMock.Clone() as MockRequestResponseModel;

        // Init retour
        Resultat = new RetourEditionModel();

        // Mode : Creation / Mise a jour / Clone
        Resultat.TypeOperation = ActionEdition;

        // Init context editor
        ContextEdit = new(Model);

        // Init Model validation
        IsInvalid = ActionEdition == TypeOperationEdition.Creation;

        ContextEdit.OnFieldChanged += (sender, e) =>
        {
            IsInvalid = !ContextEdit.Validate();
            this.StateHasChanged();
        };
    }

    protected void Close(object args)
    {
        Resultat.Success = false;

        dialogService.Close(Resultat);
    }

    /// <summary>
    ///  Sauvegarde dans la bd via l'appel au Web Api selon le mode (création ou mise à jour)
    /// </summary>
    /// <returns></returns>
    protected async Task OnSubmit()
    {
        try
        {
            if (ContextEdit.Validate())
            {
                // Clean body request/response
                Model.RequestBody = Model.RequestBody?.Clean();
                Model.ResponseBody = Model.ResponseBody?.Clean();

                var dto = Mapper.Map<MockRequestResponseDto>(Model);

                var result = Resultat.TypeOperation == TypeOperationEdition.Creation ?
                    await MockServerApi.AddAsync(dto, CancellationToken.None) :
                    await MockServerApi.UpdateAsync(dto, CancellationToken.None);

                Resultat.RequestResponse = Mapper.Map<MockRequestResponseModel>(result);

                Resultat.Success = true;

                dialogService.Close(Resultat);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error {OnSubmit}", nameof(OnSubmit));
            Resultat.Exception = true;

            if (((Refit.ApiException)ex).Content == ErrorCodes.DUPLICATE_REQUEST)
                Resultat.Message = localizer["MsgErrorDuplicated"];

            dialogService.Close(Resultat);
        }
    }
}
