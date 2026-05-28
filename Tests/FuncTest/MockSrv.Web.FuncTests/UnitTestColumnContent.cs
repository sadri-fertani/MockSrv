using Bunit;
using MockSrv.Web.Components;
using Radzen;

namespace MockSrv.Web.FuncTests;

public class UnitTestColumnContent
{
    private const int MAX_LENGHT_UI = 20;

    [Fact]
    public void ColumnContentShouldDisplayThreePoints()
    {
        using var ctx = new TestContext();
        ctx.Services.AddRadzenComponents();

        var textOriginal = "Ceci est une chaine de longeur superieur";
        var cut = ctx.RenderComponent<ColumnContent>
            (
                parameters => parameters
                    .Add(p => p.OriginalContent, textOriginal)
                    .Add(p => p.MaxLengthUI, MAX_LENGHT_UI)
            );

        var paraElm = cut.FindAll("span");
        
        if (paraElm.Count() == 2)
        {
            Assert.Equal(textOriginal.Substring(0, MAX_LENGHT_UI), paraElm.First().TextContent);
            Assert.Equal("<b>...</b>", paraElm.Last().InnerHtml);
        }
        else
        {
            Assert.Fail();
        }
    }

    [Fact]
    public void ColumnContentShouldNotDisplayThreePoints()
    {
        using var ctx = new TestContext();
        ctx.Services.AddRadzenComponents();

        var textOriginal = "petit test.";
        var cut = ctx.RenderComponent<ColumnContent>
            (
                parameters => parameters.Add(p => p.OriginalContent, textOriginal)
            );

        var paraElm = cut.FindAll("span");

        if (paraElm.Count() == 1)
        {
            Assert.Equal(textOriginal, paraElm.First().TextContent);
        }
        else
        {
            Assert.Fail();
        }
    }
}