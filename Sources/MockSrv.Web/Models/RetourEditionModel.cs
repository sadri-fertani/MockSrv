namespace MockSrv.Web.Models;

public class RetourEditionModel
{
    public bool Success { get; set; }

    public bool Exception { get; set; } = false;

    public string? Message { get; set; }

    public TypeOperationEdition TypeOperation { get; set; }

    public MockRequestResponseModel? RequestResponse { get; set; }
}

public enum TypeOperationEdition
{
    Modification,
    Creation,
    Clone
}
