using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Satelite
{
    public class SateliteSensor : MonoBehaviour, IRobotPartsSensor, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private IPduWriter pdu_writer;
        private PduIoConnector pdu_io;

        public void Initialize(object root)
        {
            if (this.root != null)
            {
                return;
            }
            this.root = (GameObject)root;
            this.root_name = string.Copy(this.root.transform.name);
            this.pdu_io = PduIoConnector.Get(this.root_name);
            this.pdu_writer = this.pdu_io.GetWriter(this.root_name + "_imu_sensorPdu");
            if (this.pdu_writer == null)
            {
                throw new ArgumentException("can not found pico_sensor pdu:" + this.root_name + "_imu_sensorPdu");
            }
        }
        public void UpdateSensorValues()
        {
            Vector3 linear = this.root.transform.position;
            Vector3 angular = this.root.transform.localEulerAngles;
            this.pdu_writer.GetWriteOps().Ref("linear").SetData("x", (double)linear.x);
            this.pdu_writer.GetWriteOps().Ref("linear").SetData("y", (double)linear.y);
            this.pdu_writer.GetWriteOps().Ref("linear").SetData("z", (double)linear.z);

            this.pdu_writer.GetWriteOps().Ref("angular").SetData("x", (double)angular.x);
            this.pdu_writer.GetWriteOps().Ref("angular").SetData("y", (double)angular.y);
            this.pdu_writer.GetWriteOps().Ref("angular").SetData("z", (double)angular.z);
            //Debug.Log("pos=" + linear);
            //Debug.Log("ang=" + angular);
        }
        public string topic_type = "geometry_msgs/Twist";
        public string topic_name = "imu_sensor";
        public int update_cycle = 1;
        public IoMethod io_method = IoMethod.SHM;
        public CommMethod comm_method = CommMethod.DIRECT;
        public bool isAttachedSpecificController()
        {
            return false;
        }

        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            RoboPartsConfigData[] configs = new RoboPartsConfigData[1];
            configs[0] = new RoboPartsConfigData();
            configs[0].io_dir = IoDir.WRITE;
            configs[0].io_method = this.io_method;
            configs[0].value.org_name = this.topic_name;
            configs[0].value.type = this.topic_type;
            configs[0].value.class_name = ConstantValues.pdu_writer_class;
            configs[0].value.conv_class_name = ConstantValues.conv_pdu_writer_class;
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
            cfg[0].sub = false;
            cfg[0].pub_option = new RostopicPublisherOption();
            cfg[0].pub_option.cycle_scale = this.update_cycle;
            cfg[0].pub_option.latch = false;
            cfg[0].pub_option.queue_size = 1;
            return cfg;
        }

    }
}

