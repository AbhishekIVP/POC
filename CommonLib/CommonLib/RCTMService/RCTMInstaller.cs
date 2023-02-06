using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace RCTMService
{
    [RunInstaller(true)]
    public partial class RCTMInstaller : System.Configuration.Install.Installer
    {
        public RCTMInstaller()
        {
            InitializeComponent();
        }
    }
}
