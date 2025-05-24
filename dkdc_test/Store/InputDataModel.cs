using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Store
{
    internal class InputDataModel : Components.utils.BindableBase
    {
        double pos_kp;
        public double PosKp
        {
            get => pos_kp;
            set
            {
                Set(ref pos_kp, value);
            }
        }

        double vel_kp;
        public double VelKp
        {
            get => vel_kp;
            set
            {
                Set(ref vel_kp, value);
            }
        }

        double vel_ki;
        public double VelKi
        {
            get => vel_ki;
            set
            {
                Set(ref vel_ki, value);
            }
        }

        public void UpdateInputData(uint id, double value)
        {
            // implement
            // This method should update the input data for the given id with the provided value.
            // It might involve updating a collection or notifying other components of the change.

            switch (id)
            {
                case 0:
                    this.PosKp = value;
                    break;
                case 1:
                    this.VelKp = value;
                    break;
                case 2:
                    this.VelKi = value;
                    break;
            }
        }

        public double GetInputValue(uint id)
        {
            // implement
            switch (id)
            {
                case 0:
                    return this.pos_kp;
                case 1:
                    return this.vel_kp;
                case 2:
                    return this.vel_ki;
            }
            return 0;
        }
    }
}
