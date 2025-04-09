using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace estadoBateria
{
    public partial class estadoBateria: ServiceBase
    {
        public estadoBateria()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("estadoBateria")) {
                System.Diagnostics.EventLog.CreateEventSource(
                    "estadoBateria", "Application");
            }
            eventLog1.Source = "estadoBateria";
            eventLog1.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Inicio Monitoreo bateria");
        }
 
        protected override void OnStop()
        {
            eventLog1.WriteEntry("Pausa Monitoreo bateria");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string Porcentaje;

            Porcentaje = (SystemInformation.PowerStatus.BatteryLifePercent * 100).ToString() + "%";
            eventLog1.WriteEntry("Porsentaje actual de la bateria: " + Porcentaje);
        }
    }
}
