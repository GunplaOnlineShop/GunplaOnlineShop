﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GunplaOnlineShop.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment) : base(context, userManager)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult OrderHistory()
        {
            return View();
        }

        public IActionResult Security()
        {
            return View();
        }

        public IActionResult Addresses()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            if (userid == null)
            {
                return RedirectToAction("Login","Account");
            }
            else 
            {
                var user = _userManager.Users
                    .Include(x => x.MailingAddresses)
                    .Where(i => i.Id == userid)
                    .FirstOrDefault();

                var model = new AddressViewModel();
                if (user.MailingAddresses != null)
                {
                    model.Addresses = user.MailingAddresses.ToList();
                }
                return View(model);
            } 
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MailingAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                user.IsAdmin = false;
                var newAddress = new MailingAddress
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Company = model.Company,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    Country = model.Country,
                    Province = model.Province,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    PhoneNumber = model.PhoneNumber,
                    IsDefaultAddress = model.IsDefaultAddress,
                    CustomerId = userId,
                };
                
                _context.MailingAddresses.Add(newAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Customer");
                /*
                List<MailingAddress> a = new List<MailingAddress>();
                if (user.MailingAddresses != null)
                {
                    var newOne = new List<MailingAddress> { new MailingAddress()
                    {FirstName = model.FirstName,
                    LastName = model.LastName,
                    Company = model.Company,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    Country = model.Country,
                    Province = model.Province,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    PhoneNumber = model.PhoneNumber,
                    IsDefaultAddress = model.IsDefaultAddress,
                    CustomerId = userId,} };
                    user.MailingAddresses = newOne;
                    user.PhoneNumber = newAddress.PhoneNumber.ToString();
                }
                else
                {
                    var newOne = new List<MailingAddress> { new MailingAddress() 
                    { Id = newAddress.Id, } };
                    user.MailingAddresses = newOne;
                    user.PhoneNumber = newAddress.PhoneNumber.ToString();
                }*
                IdentityResult result = await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }*/
            }
            return View(model);

        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Addresses", "Customer");
            }

            var mailingAddresses = _context.MailingAddresses
                .Where(i => i.CustomerId == _userManager.GetUserId(HttpContext.User));
            MailingAddress target = mailingAddresses.
                Where(i => i.Id == id)
                .FirstOrDefault();
            _context.MailingAddresses.Remove(target);
            _context.SaveChanges();
            return RedirectToAction("Addresses", "Customer");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Addresses", "Customer");
            }
            var mailingAddress = _context.MailingAddresses
                .Where(i => i.CustomerId == _userManager.GetUserId(HttpContext.User));
            MailingAddress model = mailingAddress
                .Where(i => i.Id == id)
                .FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(MailingAddress model)
        {
            if (ModelState.IsValid)
            {
                var mailingAddresses = _context.MailingAddresses
                    .Where(i => i.CustomerId == _userManager.GetUserId(HttpContext.User));
                MailingAddress preUpdate = mailingAddresses
                    .Where(i => i.Id == model.Id)
                    .FirstOrDefault();
                _context.MailingAddresses.Remove(preUpdate);
                _context.SaveChanges();
                _context.MailingAddresses.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Addresses", "Customer");
            }

            return View(model);
        }

    }
}