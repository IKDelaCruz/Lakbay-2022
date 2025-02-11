namespace Project.Models.Models
{
    public class ReturnValue
    {
        public ReturnValue(string message = "", bool sucess = false, object returnParam = null)
        {
            Success = sucess;
            Message = message;
            ReturnParam = returnParam;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object ReturnParam { get; set; }
    }


    public class ReturnValue<T> where T : class, new()
    {
        public ReturnValue() { }

        public ReturnValue(string message, bool success = false, T tRet = null)
        {
            Success = success;
            Message = message;
            ReturnData = tRet;
        }

        public ReturnValue(ReturnValue retVal, T tRet = null) : this(retVal.Message, retVal.Success,
            tRet)
        { }

        public bool Success { get; set; }
        public string Message { get; set; }
        public T ReturnData { get; set; }
    }
}
