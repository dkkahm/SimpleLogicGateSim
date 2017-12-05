using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLogicGateSim
{
    class Port
    {
        public Port(bool is_input)
        {
            m_is_input = is_input;
        }

        public bool m_is_input = false;
        public bool m_state = false;
        public HashSet<Port> m_linked_port_list = new HashSet<Port>();
        public Gate m_gate;
        public bool m_is_state_set = false;

        public void SetInput(bool is_input)
        {
            if (m_is_input != is_input)
            {
                m_is_input = is_input;
                if (m_gate != null)
                {
                    m_gate.OnPortDirectionChanged(this);
                }
            }
        }

        public bool IsInput()
        {
            return m_is_input;
        }

        public void SetState(bool state)
        {
            if (!m_is_state_set || m_state != state)
            {
                m_is_state_set = true;
                m_state = state;

                if (m_gate != null)
                {
                    m_gate.OnPortStateChanged(this);
                }
            }
        }

        public bool GetState()
        {
            return m_state;
        }

        public void LinkTo(Port port)
        {
            if (!m_linked_port_list.Contains(port))
            {
                m_linked_port_list.Add(port);

                if (m_gate != null)
                {
                    m_gate.OnPortLinkChanged(this, port, true);
                }
            }
        }

        public void DeleteLinkTo(Port port)
        {
            if (m_linked_port_list.Contains(port))
            {
                m_linked_port_list.Remove(port);
                if (m_gate != null)
                {
                    m_gate.OnPortLinkChanged(this, port, false);
                }
            }
        }

        public HashSet<Port> GetLinkedPortList()
        {
            return m_linked_port_list;
        }

        public Gate GetGate()
        {
            return m_gate;
        }

        public void SetGate(Gate gate)
        {
            if (m_gate != gate)
            {
                if (m_gate != null)
                {
                    m_gate.OnPortDetached(this);
                }

                m_gate = gate;

                if (m_gate != null)
                {
                    m_gate.OnPortAttached(this);
                }
            }
        }

        public virtual bool IsStateSet()
        {
            return m_is_state_set;
        }

        public void ClearStateSet()
        {
            m_is_state_set = false;
        }
    }

    abstract class Gate
    {
        public List<Port> m_port_list = new List<Port>();
        protected List<Port> m_input_port_list = new List<Port>();
        protected List<Port> m_output_port_list = new List<Port>();
        protected bool m_do_bake = true;
        private bool m_got_state_changed_port = false;

        abstract public void RunLogic();

        public Port GetPort(int index)
        {
            if (index >= 0 && index < m_port_list.Count)
            {
                return m_port_list[index];
            }

            return null;
        }

        public Port GetInputPort(int index)
        {
            if (index >= 0 && index < m_input_port_list.Count)
            {
                return m_input_port_list[index];
            }

            return null;
        }

        public Port GetOutputPort(int index)
        {
            if (index >= 0 && index < m_output_port_list.Count)
            {
                return m_output_port_list[index];
            }

            return null;
        }

        public void OnPortDetached(Port port)
        {
            Bake();
        }

        public void OnPortAttached(Port port)
        {
            Bake();
        }

        public void OnPortDirectionChanged(Port port)
        {
            Bake();
        }

        public void OnPortStateChanged(Port port)
        {
            m_got_state_changed_port = true;
        }

        public void OnPortLinkChanged(Port from_port, Port to_port, bool linked)
        {

        }

        public int GetInputPortCount()
        {
            return m_input_port_list.Count;
        }

        public int GetOutputPortCount()
        {
            return m_output_port_list.Count;
        }

        public void BlockBaking()
        {
            m_do_bake = false;
        }

        public void UnlockBaking()
        {
            m_do_bake = true;
        }

        public virtual void Bake()
        {
            if (m_do_bake)
            {
                m_input_port_list.Clear();
                m_output_port_list.Clear();

                foreach (Port port in m_port_list)
                {
                    if (port.IsInput())
                    {
                        m_input_port_list.Add(port);
                    }
                    else
                    {
                        m_output_port_list.Add(port);
                    }
                }
            }
        }

        public virtual void ClearDetermined()
        {
            foreach (Port port in m_input_port_list)
            {
                port.ClearStateSet();
            }
        }

        public virtual bool IsDetermined()
        {
            foreach (Port port in m_input_port_list)
            {
                if (!port.IsStateSet())
                {
                    return false;
                }
            }

            return true;
        }

        public void ClearGotStateChangedPort()
        {
            m_got_state_changed_port = false;
        }

        public bool GotStateChangedPort()
        {
            return m_got_state_changed_port;
        }

        public virtual void OnBeforeTick()
        {

        }

        public virtual void OnAfterTick()
        {

        }
    }

    abstract class BinaryBaseGate : Gate
    {
        public BinaryBaseGate()
        {
            Port port;

            this.BlockBaking();

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);

            this.UnlockBaking();
            this.Bake();
        }
    }

    class AndGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState() & m_port_list[1].GetState();

            m_port_list[2].SetState(ret);
        }
    }

    class OrGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState() | m_port_list[1].GetState();

            m_port_list[2].SetState(ret);
        }
    }

    class XorGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState() ^ m_port_list[1].GetState();

            m_port_list[2].SetState(ret);
        }
    }

    class NotGate : Gate
    {
        public NotGate()
        {
            Port port;

            this.BlockBaking();

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);

            this.UnlockBaking();
            this.Bake();
        }

        public override void RunLogic()
        {
            bool ret = !m_port_list[0].GetState();

            m_port_list[1].SetState(ret);
        }
    }

    class BufferGate : Gate
    {
        public BufferGate()
        {
            Port port;

            this.BlockBaking();

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);

            this.UnlockBaking();
            this.Bake();
        }

        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState();

            m_port_list[1].SetState(ret);
        }
    }

    class OutputOnlyGate : Gate
    {
        public OutputOnlyGate()
        {
            Port port;

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);
        }

        public override void RunLogic()
        {
        }

        public override bool IsDetermined()
        {
            return true;
        }
    }

    class AlwaysFalseGate : OutputOnlyGate
    {
        public AlwaysFalseGate()
        {
            m_port_list[0].SetState(false);
        }
    }

    class AlwaysTrueGate : OutputOnlyGate
    {
        public AlwaysTrueGate()
        {
            m_port_list[0].SetState(true);
        }
    }

    class InputOnlyGate : Gate
    {
        public InputOnlyGate()
        {
            Port port;

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);
        }

        public override void RunLogic()
        {
        }
    }

    class Clock : OutputOnlyGate
    {
        public Clock(int freq = 1)
        {
            m_freq = freq;
        }

        private int m_freq = 1;
        private int m_ticks = 0;

        public override void RunLogic()
        {
            ++m_ticks;
            if (m_ticks == m_freq)
            {
                m_port_list[0].SetState(!m_port_list[0].GetState());
                m_ticks = 0;
            }
        }

        public int GetFreq()
        {
            return m_freq;
        }

        public void SetFreq(int freq)
        {
            if (freq != m_freq)
            {
                m_freq = freq;
            }
        }
    }

    class IC : Gate
    {
        private List<Gate> m_gate_list;

        public IC(List<Gate> gate_list, List<Port> port_list)
        {
            m_gate_list = gate_list;
            m_port_list = port_list;

            this.BlockBaking();

            foreach (Port port in m_port_list)
            {
                port.SetGate(this);
            }

            this.UnlockBaking();
            this.Bake();
        }

        public override void RunLogic()
        {
            foreach (Port port in m_input_port_list)
            {
                foreach (Port linked_port in port.GetLinkedPortList())
                {
                    linked_port.SetState(port.GetState());
                }
            }

            HashSet<Gate> undetermined_list = new HashSet<Gate>(m_gate_list);
            LogicSimulator.RunTickCore(undetermined_list);

            foreach (Port port in m_output_port_list)
            {
                foreach (Port linked_port in port.GetLinkedPortList())
                {
                    linked_port.SetState(port.GetState());
                }
            }
        }

        public override void ClearDetermined()
        {
            base.ClearDetermined();

            foreach (Gate gate in m_gate_list)
            {
                gate.ClearDetermined();
            }
        }
    }

    class LogicSimulator
    {
        static public bool RunTick(List<Gate> gate_list)
        {
            foreach (Gate gate in gate_list)
            {
                gate.OnBeforeTick();
            }

            HashSet<Gate> undetermined_list = new HashSet<Gate>(gate_list);
            foreach (Gate gate in undetermined_list)
            {
                gate.ClearDetermined();
            }

            RunTickCore(undetermined_list);

            foreach (Gate gate in gate_list)
            {
                gate.OnAfterTick();
            }

            return undetermined_list.Count == 0;
        }

        static public void RunTickCore(HashSet<Gate> undetermined_list)
        {
            List<Gate> determined_list = new List<Gate>();
            List<Gate> new_determined_list = new List<Gate>();

            while (undetermined_list.Count > 0)
            {
                // 상태가 결정된 gate를 모은다.
                new_determined_list.Clear();
                foreach (Gate gate in undetermined_list)
                {
                    if (gate.IsDetermined())
                    {
                        new_determined_list.Add(gate);
                    }
                }

                foreach (Gate gate in new_determined_list)
                {
                    undetermined_list.Remove(gate);
                }

                // 변경된 상태가 있는지 체크할 플래그를 클리어한다.
                foreach (Gate gate in undetermined_list)
                {
                    gate.ClearGotStateChangedPort();
                }

                // 로직을 돌린다.
                foreach (Gate gate in new_determined_list)
                {
                    gate.RunLogic();

                    int output_port_count = gate.GetOutputPortCount();
                    for (int index = 0; index < output_port_count; ++index)
                    {
                        Port port = gate.GetOutputPort(index);

                        HashSet<Port> linked_port_list = port.GetLinkedPortList();
                        foreach (Port linked_port in linked_port_list)
                        {
                            linked_port.SetState(port.GetState());
                        }
                    }
                }

                // 변경된 상태가 있는지 확인한다.
                bool got_state_changed_gate = false;
                foreach (Gate gate in undetermined_list)
                {
                    if (gate.GotStateChangedPort())
                    {
                        got_state_changed_gate = true;
                        break;
                    }
                }
                if (!got_state_changed_gate) break;

                // 상태가 확인된 게이트를 업데이트한다.
                foreach (Gate gate in new_determined_list)
                {
                    determined_list.Add(gate);
                }
            }
        }
    }
}
