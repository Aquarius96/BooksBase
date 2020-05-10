using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace BooksBase.Shared
{
    public class Result
    {
        public bool Success;
        public IEnumerable<ErrorMessage> Errors { get; set; }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>()
            {
                Success = true,
                Value = value
            };
        }

        public static Result Ok()
        {
            return new Result()
            {
                Success = true
            };
        }

        public static Result Error(string message)
        {
            return new Result()
            {
                Success = false,
                Errors = new List<ErrorMessage>()
                {
                    new ErrorMessage()
                    {
                        PropertyName = string.Empty,
                        Message = message
                    }
                }
            };
        }

        public static Result Error(string message, string property)
        {
            return new Result()
            {
                Success = false,
                Errors = new List<ErrorMessage>()
                {
                    new ErrorMessage()
                    {
                        PropertyName = property,
                        Message = message
                    }
                }
            };
        }

        public static Result<T> Error<T>(string message)
        {
            return new Result<T>
            {
                Success = false,
                Errors = new List<ErrorMessage>()
                {
                    new ErrorMessage()
                    {
                        PropertyName = string.Empty,
                        Message = message
                    }
                }
            };
        }

        public static T ErrorResult<T>(string message) where T : Result, new()
        {
            return new T()
            {
                Success = false,
                Errors = new List<ErrorMessage>()
                {
                    new ErrorMessage()
                    {
                        PropertyName = string.Empty,
                        Message = message
                    }
                }
            };
        }

        public static Result<T> Error<T>(IEnumerable<ValidationFailure> validationFailures)
        {
            var result = new Result<T>
            {
                Success = false,
                Errors = validationFailures.Select(v => new ErrorMessage()
                {
                    PropertyName = v.PropertyName,
                    Message = v.ErrorMessage
                })
            };


            return result;
        }

        public static Result<T> Error<T>(IEnumerable<IdentityError> identityErrors)
        {
            var result = new Result<T>
            {
                Success = false,
                Errors = identityErrors.Select(v => new ErrorMessage()
                {
                    Message = v.Description
                })
            };


            return result;
        }
        public static Result Error(IEnumerable<IdentityError> identityErrors)
        {
            var result = new Result
            {
                Success = false,
                Errors = identityErrors.Select(v => new ErrorMessage()
                {
                    Message = v.Description
                })
            };


            return result;
        }
        public static Result Error(ModelStateDictionary dictionary)
        {
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            foreach (var modelState in dictionary)
            {
                var errors = modelState.Value.Errors.Select(v => new ErrorMessage()
                {
                    Message = v.ErrorMessage,
                    PropertyName = modelState.Key
                });
                errorMessages.AddRange(errors);
            }
            var result = new Result()
            {
                Success = false,
                Errors = errorMessages,
            };
            return result;
        }

        public static Result Combine(params Result[] errors)
        {
            return errors.Aggregate((success: true, errors: new List<ErrorMessage>()), (acc, r) =>
            {
                acc.success &= r.Success;
                acc.errors.AddRange(r.Errors);
                return acc;
            }, result => new Result { Success = result.success, Errors = result.errors });
        }

        public static Result<T> Error<T>(IEnumerable<ErrorMessage> errors)
        {
            return new Result<T>
            {
                Success = false,
                Errors = errors.ToList()
            };
        }
    }

    public class Result<T> : Result
    {
        public T Value;
    }

    public class ErrorMessage
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
}
