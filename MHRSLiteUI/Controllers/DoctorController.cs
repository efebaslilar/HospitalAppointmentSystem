using MHRSLiteBusinessLayer.Contracts;
using MHRSLiteEntityLayer.IdentityModels;
using MHRSLiteEntityLayer.Models;
using MHRSLiteEntityLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MHRSLiteUI.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;


        public DoctorController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public JsonResult GetDoctorsByHospitalClinic(int clinicid, int hospitalid)
        {
            try
            {
                var data = new List<DoctorViewModel>();
                var hospitalclinicdata = new List<HospitalClinic>();
                if (clinicid > 0 && hospitalid > 0)
                {
                    hospitalclinicdata = _unitOfWork.HospitalClinicRepository
                        .GetAll(x => x.ClinicId == clinicid && x.HospitalId == hospitalid
                        , includeProperties: "Doctor")
                        .Distinct().ToList();
                    // bizim sistemde sadece ertesi güne randevu verilmektedir.
                    // bu nedenle yukarıdaki data içindeki tek tek gezip hangisinin boş 
                    // randevusu varsa o doktorları gönderelim. 
                    var tomorrow = Convert.ToDateTime(DateTime.Now.AddDays(1).ToShortDateString());

                    foreach (var item in hospitalclinicdata)
                    {
                        var appointmentHourList = _unitOfWork.AppointmentHourRepository
                             .GetAll(x => x.HospitalClinicId == item.Id);

                        foreach (var appointmentItem in appointmentHourList) // tablodaki veriler
                        {
                            foreach (var hourItem in appointmentItem.Hours.Split(',')) // verinin içindeki muayene saatleri
                            {
                                var appointmentCount = _unitOfWork.AppointmentRepository
                           .GetAll(x => x.HospitalClinicId == item.Id
                           && x.AppointmentDate == tomorrow && x.AppointmentHour == hourItem).Count();
                                if (appointmentCount == 0)
                                {
                                    //appointmentCount sıfır ise demekki yarına randevusu boştur
                                    //doctoru bul ve ekle
                                    var doctor = _userManager.FindByIdAsync(item.Doctor.UserId).Result;
                                    if (data.Count(x=> x.TCNumber==item.DoctorId)==0)
                                    {
                                        data.Add(new DoctorViewModel()
                                        {
                                            UserId = item.Doctor.UserId,
                                            TCNumber = item.DoctorId,
                                            Name = doctor.Name,
                                            Surname = doctor.Surname
                                        });
                                    }
                                }
                            }
                        }
                    }
                    
                }
                return Json(new { isSuccess = true, data});

            }
            catch (Exception)
            {

                return Json(new { isSuccess = false });

            }

        }
    }
}
