using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Satelite
{
    public class SateliteController : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;
        public string topic_type = "geometry_msgs/Twist";
        public string topic_name = "cmd_vel";
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        private Rigidbody SateliterRig = null;

        public void Initialize(object obj)
        {
            GameObject tmp = null;
            try
            {
                tmp = obj as GameObject;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Initialize error: " + e.Message);
                return;
            }

            if (this.root == null)
            {
                this.root = tmp;
                this.root_name = string.Copy(this.root.transform.name);
                this.pdu_io = PduIoConnector.Get(root_name);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }
                var pdu_reader_name = root_name + "_" + this.topic_name + "Pdu";
                this.pdu_reader = this.pdu_io.GetReader(pdu_reader_name);
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_reader_name);
                }
                SateliterRig = this.root.GetComponent<Rigidbody>();
                if (this.SateliterRig == null)
                {
                    throw new ArgumentException("can not found SateliterRig:" + root_name);
                }
            }
            //Vector3 l_force = new Vector3(0, 0.1f, 0.1f);
            //SateliterRig.AddForce(l_force, ForceMode.Impulse);
        }
        public void DoControl()
        {
            float l_x = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("x");
            float l_y = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("y");
            float l_z = (float)this.pdu_reader.GetReadOps().Ref("linear").GetDataFloat64("z");
            float a_x = (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("x");
            float a_y = (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("y");
            float a_z = (float)this.pdu_reader.GetReadOps().Ref("angular").GetDataFloat64("z");

            Vector3 l_force = new Vector3(l_x, l_y, l_z);
            SateliterRig.AddForce(l_force, ForceMode.Impulse);
        }

        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.READ;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name;
            configs[0].value.type = this.topic_type;
            configs[0].value.class_name = ConstantValues.pdu_reader_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_reader_class;
            configs[0].value.pdu_size = ConstantValues.Twist_pdu_size;
            configs[0].value.write_cycle = this.update_cycle;
            configs[0].value.method_type = this.comm_method.ToString();
            return configs;
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            RosTopicMessageConfig[] cfg = new RosTopicMessageConfig[1];
            cfg[0] = new RosTopicMessageConfig();
            cfg[0].topic_message_name = this.topic_name;
            cfg[0].topic_type_name = this.topic_type;
            cfg[0].sub = true;
            return cfg;
        }

    }
}
