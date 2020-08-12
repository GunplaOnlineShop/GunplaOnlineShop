using GunplaOnlineShop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GunplaOnlineShop.ViewModels
{
    public class PhotoEditViewModel
    {
        public int ItemId { get; set; }
        public IFormFile CoverUpdate { get; set; }
        public List<IFormFile> GalleryUpdate { get; set; }
        public Photo Cover { get; set; }
        public List<Photo> Gallery { get; set; }
    }
}
