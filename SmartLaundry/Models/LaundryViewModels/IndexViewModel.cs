﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Models.LaundryViewModels
{
    public class IndexViewModel
    {
        public List<Laundry> Laundries;
        public int DormitoryId;
        public Reservation currentRoomReservation;
    }
}
