using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TaskHelper.Core.Models;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace TaskHelper.Core.ViewModels
{
    public class ProjectsViewModel : BindableBase
    {
        private const string groupsRoute = "http://dynamicformapi.herokuapp.com/groups.json";
        private const string proceduresRoute = "http://dynamicformapi.herokuapp.com/procedures.json";
        private const string stepsRoute = "http://dynamicformapi.herokuapp.com/steps.json";

        private ObservableCollection<Procedure> procedures;

        public ObservableCollection<Procedure> Procedures
        {
            get {
                if (procedures==null)
                {
                    procedures = new ObservableCollection<Procedure>();
                }
                return procedures;
            }
            set { procedures = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Step> steps;

        public ObservableCollection<Step> Steps
        {
            get {
                if (steps==null)
                {
                    steps = new ObservableCollection<Step>();
                }
                return steps; }
            set { steps = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Group> groups;

        public ObservableCollection<Group> Groups
        {
            get {
                if (groups==null)
                {
                    groups = new ObservableCollection<Group>();
                }
                return groups; }
            set { groups = value; OnPropertyChanged(); }
        }

        
        private bool isDataLoaded = false;

        public bool IsDataLoaded
        {
            get { return isDataLoaded; }
            set { isDataLoaded = value; OnPropertyChanged(); }
        }

        public async Task LoadData()
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(new Uri(groupsRoute, UriKind.Absolute));

            Groups =  JsonConvert.DeserializeObject<ObservableCollection<Group>>(response);

            response = await client.GetStringAsync(new Uri(proceduresRoute, UriKind.Absolute));
            Procedures = JsonConvert.DeserializeObject<ObservableCollection<Procedure>>(response);

            response = await client.GetStringAsync(new Uri(stepsRoute, UriKind.Absolute));
            Steps = JsonConvert.DeserializeObject<ObservableCollection<Step>>(response);

            foreach (var item in Steps)
            {
                try
                {
                    item.content2 = JsonConvert.DeserializeObject<StepInfo>(item.content);
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Step Damaged: " + item.step_id + " from process: " + item.procedure_id);
                }
            }

            IsDataLoaded = true;
        }
    }
}
