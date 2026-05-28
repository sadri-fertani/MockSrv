using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MockSrv.Api.UnitTests;

[ExcludeFromCodeCoverage]
public static class SqlExceptionFactory
{
    public static SqlException Create(int number)
    {
        Exception? innerEx = null;
        var c = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        
        SqlErrorCollection errors = (c[0].Invoke(null) as SqlErrorCollection)!;
        
        var errorList = (errors.GetType().GetField("_errors", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(errors) as List<object>)!;
        c = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        var nineC = c.FirstOrDefault(f => f.GetParameters().Length == 9)!;
        
        SqlError sqlError = (nineC.Invoke(new object?[] { number, (byte)0, (byte)0, "", "", "", (int)0, (uint)0, innerEx }) as SqlError)!;
        errorList.Add(sqlError);
        
        SqlException ex = (Activator.CreateInstance
            (
                typeof(SqlException), 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new object?[] 
                { 
                    "test", 
                    errors,
                    innerEx, 
                    Guid.NewGuid() 
                }, 
                null
            ) as SqlException)!;
        
        return ex;
    }
}
