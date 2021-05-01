using System;
using System.ComponentModel;

namespace Glue.Delivery.Core.Domain
{
    public sealed class OperationResult<T>
    {
        private OperationResult()
        {
        }

        public T Result { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool Failed { get; private set; }
        public OperationErrorReason ErrorReason { get; private set; } = OperationErrorReason.None;
        public bool Succeed => !Failed;

        public static OperationResult<T> Success(T result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return new OperationResult<T> {Result = result, Failed = false};
        }

        public static OperationResult<T> Error(string errorMessage)
        {
            return Error(OperationErrorReason.GenericError, errorMessage);
        }

        public static OperationResult<T> Error(OperationErrorReason errorReason, string errorMessage)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            if (!Enum.IsDefined(typeof(OperationErrorReason), errorReason))
                throw new InvalidEnumArgumentException(nameof(errorReason), (int) errorReason,
                    typeof(OperationErrorReason));

            return new OperationResult<T> {ErrorMessage = errorMessage, ErrorReason = errorReason, Failed = true};
        }
    }
}