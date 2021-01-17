using System;

namespace OnlineShopping.Services.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) :
            base(message)
        {

        }
    }
}
