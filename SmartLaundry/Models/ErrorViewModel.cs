namespace SmartLaundry.Models
{
    public class ErrorViewModel
    {
        private string _title;
        private string _message;

        public string Message
        {
            get => string.IsNullOrEmpty(_message) ? "An error occurred while processing your request." : _message;
            set => _message = value;
        }

        public string Title
        {
            get => string.IsNullOrEmpty(_title) ? "Error" : _title;
            set => _title = value;
        }
    }
}