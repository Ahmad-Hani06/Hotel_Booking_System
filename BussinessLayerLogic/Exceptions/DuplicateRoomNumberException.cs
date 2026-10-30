using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayerLogic.Exceptions
{
    public class DuplicateRoomNumberException : Exception
    {
        public DuplicateRoomNumberException(string roomNumber): base($"Room number {roomNumber} already exists.")
        {

        }
    }
}
