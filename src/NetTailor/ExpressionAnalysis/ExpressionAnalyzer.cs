using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace NetTailor.ExpressionAnalysis;

internal static class ExpressionAnalyzer
{
    public static void Analyze<TParameter, TResult>(Expression<Func<TParameter, TResult>> configure)
    {
        var sb = new StringBuilder();
        if (configure.Body is UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MemberExpression memberExpression)
            {
                sb.Append($"\tThe boxed member is: {memberExpression.Member.Name},\n\tthe member type is: {memberExpression.Type}");
            }
            else
            {
                sb.Append($"\nThe operand is not a member expression, actual type is {configure.Body.Type}");
            }
        }
        if (configure.Body is NewExpression newExpression)
        {
            sb.Append($"\tThe new expression type is: {newExpression.Type}");
            sb.Append("\n\tThe new expression arguments are:");
            foreach (var argument in newExpression.Arguments)
            {
                sb.Append($"\n\t > {argument.Type}");
            }
            sb.Append("\n\tThe new expression members are:");
            foreach (var argument in newExpression.Members)
            {
                sb.Append($"\n\t > {argument.Name}: {argument.MemberType}");
            }
        }
        else
        {
            sb.Append($"\nThe body is not a unary expression, actual type is {configure.Body.Type}");
        }
        
        Debug.WriteLine(sb.ToString());
    }
    
    public static void AnalyzeAction<TParam1, TParam2>(Action<TParam1, TParam2> action)
    {
        var sb = new StringBuilder();
        sb.Append($"\tThe method name is: {action.Method.Name}");
        sb.Append($"\n\tThe method return type is: {action.Method.ReturnType}");
        sb.Append("\n\tThe method parameters are:");
        foreach (var parameter in action.Method.GetParameters())
        {
            sb.Append($"\n\t > {parameter.ParameterType} {parameter.Name}");
        }
        Debug.WriteLine(sb.ToString());
    }
}