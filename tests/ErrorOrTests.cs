namespace UnitTests;

using ErrorOr;
using FluentAssertions;

public class ErrorOrTests
{
    private record Person(string Name);

    [Fact]
    public void ImplicitCastResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        var result = new Person("Amichai");

        // Act
        ErrorOr<Person> errorOr = result;

        // Assert
        errorOr.IsError.Should().BeFalse();
        errorOr.Value.Should().Be(result);
    }

    [Fact]
    public void ImplicitCastPrimitiveResult_WhenAccessingResult_ShouldReturnValue()
    {
        // Arrange
        const int result = 4;

        // Act
        ErrorOr<int> errorOrInt = result;

        // Assert
        errorOrInt.IsError.Should().BeFalse();
        errorOrInt.IsSuccess.Should().BeTrue();
        errorOrInt.Value.Should().Be(result);
    }

    [Fact]
    public void ImplicitCastErrorOrType_WhenAccessingResult_ShouldReturnValue()
    {
        // Act
        ErrorOr<Success> errorOrSuccess = Result.Success;
        ErrorOr<Created> errorOrCreated = Result.Created;
        ErrorOr<Deleted> errorOrDeleted = Result.Deleted;
        ErrorOr<Updated> errorOrUpdated = Result.Updated;

        // Assert
        errorOrSuccess.IsError.Should().BeFalse();
        errorOrSuccess.IsSuccess.Should().BeTrue();
        errorOrSuccess.Value.Should().Be(Result.Success);

        errorOrCreated.IsError.Should().BeFalse();
        errorOrCreated.IsSuccess.Should().BeTrue();
        errorOrCreated.Value.Should().Be(Result.Created);

        errorOrDeleted.IsError.Should().BeFalse();
        errorOrDeleted.IsSuccess.Should().BeTrue();
        errorOrDeleted.Value.Should().Be(Result.Deleted);

        errorOrUpdated.IsError.Should().BeFalse();
        errorOrUpdated.IsSuccess.Should().BeTrue();
        errorOrUpdated.Value.Should().Be(Result.Updated);
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        var error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.Errors.Should().ContainSingle().Which.Should().Be(error);
    }

    [Fact]
    public void ImplicitCastSingleError_WhenAccessingFirstError_ShouldReturnError()
    {
        // Arrange
        var error = Error.Validation("User.Name", "Name is too short");

        // Act
        ErrorOr<Person> errorOrPerson = error;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.FirstError.Should().Be(error);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingErrors_ShouldReturnErrorList()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.Errors.Should().HaveCount(errors.Count).And.BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingErrors_ShouldReturnErrorArray()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.Errors.Should().HaveCount(errors.Length).And.BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        var errors = new List<Error>
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.FirstError.Should().Be(errors[0]);
    }

    [Fact]
    public void ImplicitCastErrorArray_WhenAccessingFirstError_ShouldReturnFirstError()
    {
        // Arrange
        var errors = new[]
        {
            Error.Validation("User.Name", "Name is too short"),
            Error.Validation("User.Age", "User is too young"),
        };

        // Act
        ErrorOr<Person> errorOrPerson = errors;

        // Assert
        errorOrPerson.IsError.Should().BeTrue();
        errorOrPerson.IsSuccess.Should().BeFalse();
        errorOrPerson.FirstError.Should().Be(errors[0]);
    }

    [Fact]
    public void ImplicitCastErrorList_WhenProvidingEmptyListShouldThrow()
    {
        // Arrange
        var errors = new List<Error>();

        // Act
        var errorOrCreation = () =>
        {
            ErrorOr<Person> errorOrPerson = errors;
        };

        // Assert
        errorOrCreation.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StaticCreationErrorList_WhenProvidingEmptyListShouldThrow()
    {
        // Arrange
        var errors = new List<Error>();

        // Act
        var errorOrCreation = () =>
        {
            ErrorOr<Person> errorOrPerson = ErrorOr<Person>.Fail(errors);
        };

        // Assert
        errorOrCreation.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StaticResultCreation_ShouldBePossible()
    {
        // Act
        ErrorOr<Success> errorOrSuccess = ErrorOr.Ok(Result.Success);

        // Assert
        errorOrSuccess.IsError.Should().BeFalse();
        errorOrSuccess.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void StaticErrorCreation_ShouldBePossible()
    {
        // Arrange
        var error = Error.Validation("User.Name", "Name is too short");
        var errorArray = new[] { error, error };
        var errorList = new List<Error> { error, error };

        // Act
        ErrorOr<bool> oneError = ErrorOr<bool>.Fail(error);
        ErrorOr<bool> twoErrorsWithParams = ErrorOr<bool>.Fail(error, error);
        ErrorOr<bool> threeErrorsWithParams = ErrorOr<bool>.Fail(error, error, error);
        ErrorOr<bool> twoErrorsWithArray = ErrorOr<bool>.Fail(errorArray);
        ErrorOr<bool> twoErrorsWithList = ErrorOr<bool>.Fail(errorList);

        // Assert
        oneError.IsError.Should().BeTrue();
        oneError.IsSuccess.Should().BeFalse();
        oneError.Errors.Should().HaveCount(1);

        twoErrorsWithParams.IsError.Should().BeTrue();
        twoErrorsWithParams.IsSuccess.Should().BeFalse();
        twoErrorsWithParams.Errors.Should().HaveCount(2);

        threeErrorsWithParams.IsError.Should().BeTrue();
        threeErrorsWithParams.IsSuccess.Should().BeFalse();
        threeErrorsWithParams.Errors.Should().HaveCount(3);

        twoErrorsWithArray.IsError.Should().BeTrue();
        twoErrorsWithArray.IsSuccess.Should().BeFalse();
        twoErrorsWithArray.Errors.Should().HaveCount(2);

        twoErrorsWithList.IsError.Should().BeTrue();
        twoErrorsWithList.IsSuccess.Should().BeFalse();
        twoErrorsWithList.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void ErrorIsError_ShouldCompileWhenGettingValueOrErrors()
    {
        // Arrange
        ErrorOr<Person> errorOrWithError = Error.Validation("User.Name", "Name is too short");

        // Act
        if (errorOrWithError.IsError)
        {
            IReadOnlyList<Error> errors = errorOrWithError.Errors;
            Error error = errorOrWithError.FirstError;
        }
        else
        {
            Person result = errorOrWithError.Value;
        }
    }

    [Fact]
    public void ErrorIsSuccess_ShouldCompileWhenGettingValueOrErrors()
    {
        // Arrange
        ErrorOr<Person> errorOrWithResult = new Person("ThisCouldBeYourName");

        // Act
        if (errorOrWithResult.IsSuccess)
        {
            Person result = errorOrWithResult.Value;
        }
        else
        {
            IReadOnlyList<Error> errors = errorOrWithResult.Errors;
            Error error = errorOrWithResult.FirstError;
        }
    }
}
