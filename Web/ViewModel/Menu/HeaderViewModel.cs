using Project.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.ViewModel.Menu
{
    public class HeaderViewModel
    {
        private List<PlaceDto> _listCityHistory { get; set; }
        public List<PlaceDto> ListCityHistory
        {
            get => _listCityHistory;
            set
            {
                //sort, oriental mindoro first 

                var temp = value.ToList();
                var orientalMindoro = temp.FirstOrDefault(s => s.HomeSlug.ToLower().Contains("oriental-mindoro"));
                var newList = new List<PlaceDto>();
                if(orientalMindoro != null)
                {
                    newList.Add(orientalMindoro);
                    temp.Remove(orientalMindoro);
                }
                newList.AddRange(temp.OrderBy(s => s.ParentName));
                _listCityHistory = newList;
                
            }
        }
    }
}