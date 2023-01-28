using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebScheduler.Models;

namespace WebScheduler.ViewModels
{
    public class AppointmentsViewModel
    {
        public Appointment Appointment { get; set; }

        public DateTime? Date { get; set; }
    }
}
