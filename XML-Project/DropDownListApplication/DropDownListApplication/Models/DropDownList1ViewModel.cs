using DropDownListApplication.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DropDownListApplication.Models
{
    public class DropDownList1ViewModel
    {
        
        public string DropDown1 { get; set; }

       /* public static IEnumerable<SelectListItem> GetXml()
        {
            GridListController gridListController = new GridListController();
            var stores = gridListController.NodeObject();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var store in stores)
            {
                var sli = new SelectListItem();
                sli.Value = store.code;
                sli.Text = store.code;
                list.Add(sli);
            }
            return list;
        }*/

    }
}