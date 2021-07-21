using Movil.Models.ResultQry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.searchs
{
    public class SearchSKU : SearchHandler
    {
        private List<ArticulosServicio> zart { get; set; } = new List<ArticulosServicio>();
        private string zparada;
        public SearchSKU(List<ArticulosServicio> z,string p)    
        {
            zart = z;
            zparada = p;
        }
        
        
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);
            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = zart
                              .Where(s => s.SKU
                              .ToLower().Contains(newValue.ToLower())).ToList();
            }
        }

        protected override void OnQueryConfirmed()
        {
            base.OnQueryConfirmed();
        }
        protected override  void OnItemSelected(object item)
        {
            base.OnItemSelected(item);
            
            //Shell.Current.GoToAsync($"articulosporentregar?cveparada={zparada}&zsku={((ArticulosServicio)item).SKU}");

        }

    


}
}
