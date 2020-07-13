namespace gdrive
{
    class JsonReturn
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Response { get; set; }

        public JsonReturn (string status, string msg, object obj)
        {
            Status = status;
            Message = msg;
            Response = obj;
        }
    }
}
