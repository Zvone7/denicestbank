using System.ComponentModel.DataAnnotations;
using System.Data;
using LanguageExt.Common;

namespace Portal.Core.Extensions;

public static class ValidationExtensions
{
    public static Result<TToValidate> ValidateConstraints<TToValidate>(
        this TToValidate objectToValidate
    )
        where TToValidate : class
    {
        var context = new ValidationContext(objectToValidate);
        var validationResults = new List<ValidationResult>();
        var valid = Validator.TryValidateObject
            (objectToValidate, context, validationResults, true);
        if (!valid)
        {
            var errors = new List<String>();
            validationResults.ForEach(vr => { errors.Add(vr.ErrorMessage!); });
            return new Result<TToValidate>(new ValidationException(
                string.Join("\n", errors)));
        }

        return new Result<TToValidate>(objectToValidate);
    }

    public static Result<Boolean> IsNotNull<T>(this T obj)
    {
        if (obj == null)
            return new Result<Boolean>(GetIsNotNullException<T>());
        return new Result<Boolean>(true);
    }

    public static DataException GetIsNotNullException<T>()
    {
        return new DataException($"{GetNameOfT<T>()} not set.");
    }

    public static Result<Boolean> MustBeNull<T>(this T obj)
    {
        if (obj != null)
            return new Result<Boolean>(GetMustBeNullException<T>());
        return new Result<Boolean>(true);
    }

    public static DataException GetMustBeNullException<T>()
    {
        return new DataException($"{GetNameOfT<T>()} must be null.");
    }

    public static Result<Boolean> AreBothNotNull<T1, T2>(this (T1, T2) obj)
    {
        if (obj.Item1 == null && obj.Item2 == null)
            return new Result<Boolean>(GetAreBothNotNullException<T1, T2>());
        return new Result<Boolean>(true);
    }

    public static DataException GetAreBothNotNullException<T1, T2>()
    {
        return new DataException($"Either {GetNameOfT<T1>()} or {GetNameOfT<T2>()} must have value.");
    }

    public static Result<Boolean> IsNotNullOrEmpty<T>(this IEnumerable<T>? obj)
    {
        if (obj == null || !obj.Any())
            return new Result<Boolean>(GetIsNotNullOrEmptyException<T>());
        return new Result<Boolean>(true);
    }

    public static DataException GetIsNotNullOrEmptyException<T>()
    {
        return new DataException($"List of {GetNameOfT<T>()} cannot be null or empty.");
    }

    private static String GetNameOfT<T>()
    {
        var t = typeof(T);
        var name = t.Name;
        if (t.IsGenericType && t.GenericTypeArguments.Any())
        {
            name = t.GenericTypeArguments
                .Select(x => x.Name)
                .Aggregate(name, (current, genTypeArgName) => current + (" of " + genTypeArgName));
        }

        return name;
    }
}