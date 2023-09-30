using System;
using System.Collections;
using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Robot.Satelite.TestDriver
{
    public class SateliteControllerTestDriver : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private GameObject root;
        private string root_name;
        private PduIoConnector pdu_io;
        private IPduReader pdu_reader;

        public string topic_name = "cmd_vel";
        public string roboname = "Satelite";
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
                    throw new ArgumentException("can not found pdu_io:" + roboname);
                }
                var pdu_io_name = roboname + "_" + this.topic_name + "Pdu";
                this.pdu_reader = this.pdu_io.GetReader(pdu_io_name);
                if (this.pdu_reader == null)
                {
                    throw new ArgumentException("can not found pdu_reader:" + pdu_io_name);
                }
            }
        }
        public float delta_vel = 0.1f;
        public Vector3 cmd = Vector3.zero;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.cmd.y += delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.cmd.y -= delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.cmd.z += delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.cmd.z -= delta_vel;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.cmd = Vector3.zero;
            }
        }

        public double target_velocity;
        public double target_rotation_angle_rate;
        public void DoControl()
        {
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("x", (double)cmd.x);
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("y", (double)cmd.y);
            this.pdu_reader.GetWriteOps().Ref("linear").SetData("z", (double)cmd.z);
            this.cmd = Vector3.zero;
        }
        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            return new RoboPartsConfigData[0];
        }
    }
}
