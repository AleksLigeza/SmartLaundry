namespace SmartLaundry.Models
{
    public class ErrorViewModel
    {
        private string _title;
        private string _message;

        public string RequestId { get; set; }

        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? _message : "An error occurred while processing your request.";
            set => _message = value;
        }

        public string Title
        {
            get => string.IsNullOrEmpty(_title) ? _title : "Error";
            set => _title = value;
        }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}