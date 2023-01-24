using HotChocolate;

namespace WebApplication2.GraphQL;

public class ErrorFilter: IErrorFilter
{
    public IError OnError(IError error)
    {
        var message = error.Exception?.Message;

        if (message != null)
        {
            return error.WithMessage(message);
        }

        return error;
    }
}