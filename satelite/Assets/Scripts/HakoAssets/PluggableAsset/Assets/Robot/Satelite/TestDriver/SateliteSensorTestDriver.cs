using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Satelite.TestDriver
{
    public class SateliteSensorTestDriver : MonoBehaviour, IRobotPartsSensor, IRobotPartsConfig
    {
        private GameObject root;
        private PduIoConnector pdu_io;
        private IPduWriter pdu_writer;
        private string root_name;
        public int update_cycle = 1;
        public string topic_name = "imu_sensor";
        public string roboname = "Satelite";

        public Vector3 linear = Vector3.zero;
        public Vector3 angle = Vector3.zero;

        public RosTopicMessageConfig[] getRosConfig()
        {
            return new RosTopicMessageConfig[0];
        }

        public void Initialize(System.Object obj)
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
                this.pdu_io = PduIoConnector.Get(roboname);
                if (this.pdu_io == null)
                {
                    throw new ArgumentException("can not found pdu_io:" + root_name);
                }
                var pdu_writer_name = roboname + "_" + this.topic_name + "Pdu";
                this.pdu_writer = this.pdu_io.GetWriter(pdu_writer_name);
                if (this.pdu_writer == null)
                {
                    throw new ArgumentException("can not found pdu_writer:" + pdu_writer_name);
                }
            }
        }

        public bool isAttachedSpecificController()
        {
            return false;
        }

        public void UpdateSensorValues()
        {
            float l_x = (float)this.pdu_writer.GetReadOps().Ref("linear").GetDataFloat64("x");
            float l_y = (float)this.pdu_writer.GetReadOps().Ref("linear").GetDataFloat64("y");
            float l_z = (float)this.pdu_writer.GetReadOps().Ref("linear").GetDataFloat64("z");
            this.linear = new Vector3(l_x, l_y, l_z);
            float a_x = (float)this.pdu_writer.GetReadOps().Ref("angular").GetDataFloat64("x");
            float a_y = (float)this.pdu_writer.GetReadOps().Ref("angular").GetDataFloat64("y");
            float a_z = (float)this.pdu_writer.GetReadOps().Ref("angular").GetDataFloat64("z");
            this.angle = new Vector3(a_x, a_y, a_z);
        }
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            return new RoboPartsConfigData[0];
        }
    }
}
