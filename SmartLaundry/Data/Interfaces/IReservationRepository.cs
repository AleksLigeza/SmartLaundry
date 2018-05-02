﻿using SmartLaundry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Data.Interfaces
{
    public interface IReservationRepository
    {
        IQueryable<Reservation> Reservations { get; }

        Reservation GetReservationById(int id);
        List<Reservation> GetRoomReservations(int roomId);
        Reservation GetRoomTodaysReservation(int roomId);
        void AddReservation(Reservation reservation);
        void RemoveReservation(Reservation reservation);
        void UpdateRescervation(Reservation reservation);
        Reservation GetHourReservation(int machineId, DateTime startTime);
        bool IsFaultAtTime(int machineId, DateTime time);
        bool IsFaultAtTimeToday(int machineId, int hour);
        bool IsCurrentlyFault(int machineId);
    }
}
