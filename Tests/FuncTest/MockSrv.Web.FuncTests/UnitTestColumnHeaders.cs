using Bunit;
using MockSrv.Web.Components;
using Radzen;

namespace MockSrv.Web.FuncTests;

public class UnitTestColumnHeaders
{
    [Fact]
    public void ColumnHeadersShouldDisplayTwoDiv()
    {
        using var ctx = new TestContext();
        ctx.Services.AddRadzenComponents();

        var textOriginal = "k1=v1&k2=v2&k3=v3";
        var cut = ctx.RenderComponent<ColumnHeaders>
            (
                parameters => parameters
                    .Add(p => p.OriginalContent, textOriginal)
            );

        var paraElm = cut.FindAll("div");
        if (paraElm.Count() == 4)
        {
            Assert.Equal("k1=v1", paraElm.ElementAt(1).InnerHtml);
            Assert.Equal("k2=v2", paraElm.ElementAt(2).InnerHtml);
            Assert.Equal("k3=v3", paraElm.ElementAt(3).InnerHtml);
        }
        else
        {
            Assert.Fail();
        }
    }
}