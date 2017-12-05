using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace SimpleLogicGateSim
{
    class Port
    {
        public Port(bool is_input)
        {
            m_is_input = is_input;
            m_hash_code = GetHashCode();
        }

        public bool m_is_input = false;
        public bool m_state = false;
        public HashSet<Port> m_linked_port_list = new HashSet<Port>();
        public Gate m_gate;
        public bool m_is_state_set = false;
        public string m_name;
        public int m_hash_code = -1;

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

        public void SetName(string name)
        {
            m_name = name;
        }

        public string GetName()
        {
            return m_name;
        }

        public Port Clone()
        {
            Port clone_port = new Port(this.m_is_input);
            clone_port.SetName(this.m_name);
            return clone_port;
        }
    }

    abstract class Gate
    {
        protected List<Port> m_port_list = new List<Port>();
        protected List<Port> m_input_port_list = new List<Port>();
        protected List<Port> m_output_port_list = new List<Port>();
        protected bool m_do_bake = true;
        private bool m_got_state_changed_port = false;
        public string m_name;
        public int m_hash_code = -1;
        protected Dictionary<String, Port> m_port_map;

        abstract public void RunLogic();

        public Gate()
        {
            m_hash_code = GetHashCode();
        }

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

        public List<Port> GetInputPortList()
        {
            return m_input_port_list;
        }

        public List<Port> GetOutputPortList()
        {
            return m_output_port_list;
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

        public int GetIndexOfPort(Port port)
        {
            return m_port_list.IndexOf(port);
        }

        public int GetIndexOfInputPort(Port port)
        {
            return m_input_port_list.IndexOf(port);
        }

        public int GetIndexOfOutputPort(Port port)
        {
            return m_output_port_list.IndexOf(port);
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

        public void SetName(string name)
        {
            m_name = name;
        }

        public string GetName()
        {
            return m_name;
        }

        public abstract Gate Clone();

        public Port FindPortById(string id)
        {
            if(m_port_map != null)
            {
                if(m_port_map.ContainsKey(id))
                {
                    return m_port_map[id];
                }
            }

            return null;
        }
    }

    abstract class BinaryBaseGate : Gate
    {
        public BinaryBaseGate()
        {
            m_port_map = new Dictionary<string, Port>();

            Port port;

            this.BlockBaking();

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("in0", port);

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("in1", port);

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("out0", port);

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

        public override Gate Clone()
        {
            Gate clone_gate = new AndGate();

            return clone_gate;
        }
    }

    class OrGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState() | m_port_list[1].GetState();

            m_port_list[2].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new OrGate();

            return clone_gate;
        }
    }

    class XorGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState() ^ m_port_list[1].GetState();

            m_port_list[2].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new XorGate();

            return clone_gate;
        }
    }

    class NandGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = !(m_port_list[0].GetState() & m_port_list[1].GetState());

            m_port_list[2].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new NandGate();

            return clone_gate;
        }
    }

    class NorGate : BinaryBaseGate
    {
        public override void RunLogic()
        {
            bool ret = !(m_port_list[0].GetState() | m_port_list[1].GetState());

            m_port_list[2].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new NorGate();

            return clone_gate;
        }
    }

    class BufferGate : Gate
    {
        public BufferGate()
        {
            m_port_map = new Dictionary<string, Port>();

            Port port;

            this.BlockBaking();

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("in0", port);

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("out0", port);

            this.UnlockBaking();
            this.Bake();
        }

        public override void RunLogic()
        {
            bool ret = m_port_list[0].GetState();

            m_port_list[1].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new BufferGate();

            return clone_gate;
        }
    }

    class NotGate : BufferGate
    {
        public override void RunLogic()
        {
            bool ret = !m_port_list[0].GetState();

            m_port_list[1].SetState(ret);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new NotGate();

            return clone_gate;
        }
    }

    class OutputOnlyGate : Gate
    {
        public OutputOnlyGate()
        {
            m_port_map = new Dictionary<string, Port>();

            Port port;

            port = new Port(false);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("out0", port);
        }

        public override void RunLogic()
        {
        }

        public override bool IsDetermined()
        {
            return true;
        }

        public override Gate Clone()
        {
            Gate clone_gate = new OutputOnlyGate();

            return clone_gate;
        }
    }

    class AlwaysFalseGate : OutputOnlyGate
    {
        public AlwaysFalseGate()
        {
            m_port_list[0].SetState(false);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new AlwaysFalseGate();

            return clone_gate;
        }
    }

    class AlwaysTrueGate : OutputOnlyGate
    {
        public AlwaysTrueGate()
        {
            m_port_list[0].SetState(true);
        }

        public override Gate Clone()
        {
            Gate clone_gate = new AlwaysTrueGate();

            return clone_gate;
        }
    }

    class InputOnlyGate : Gate
    {
        public InputOnlyGate()
        {
            m_port_map = new Dictionary<string, Port>();

            Port port;

            port = new Port(true);
            m_port_list.Add(port);
            port.SetGate(this);
            m_port_map.Add("in0", port);
        }

        public override void RunLogic()
        {
        }

        public override Gate Clone()
        {
            Gate clone_gate = new InputOnlyGate();

            return clone_gate;
        }
    }

    class ClockGate : OutputOnlyGate
    {
        public ClockGate(int freq = 1)
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

        public override Gate Clone()
        {
            Gate clone_gate = new ClockGate(m_freq);

            return clone_gate;
        }
    }

    class IC : Gate
    {
        private List<Gate> m_gate_list;

        public IC(List<Gate> gate_list, List<Port> port_list, Dictionary<String, Port> port_map = null)
        {
            m_gate_list = gate_list;
            m_port_list = port_list;
            m_port_map = port_map;

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

        public List<Gate> GetGateList()
        {
            return m_gate_list;
        }

        public override Gate Clone()
        {
            Dictionary<Gate, Gate> gate_map = new Dictionary<Gate, Gate>();

            List<Gate> clone_gate_list = new List<Gate>();
            foreach(Gate gate in m_gate_list)
            {
                Gate clone_gate = gate.Clone();
                clone_gate_list.Add(clone_gate);

                gate_map.Add(gate, clone_gate);
            }

            List<Port> clone_port_list = new List<Port>();
            foreach(Port port in m_port_list)
            {
                Port clone_port = port.Clone();
                clone_port_list.Add(clone_port);
            }

            Dictionary<string, Port> clone_port_map = new Dictionary<string, Port>();
            foreach(var kv in m_port_map)
            {
                Port port = kv.Value;
                int port_index = m_port_list.IndexOf(port);
                clone_port_map.Add(kv.Key, clone_port_list[port_index]);
            }

            foreach (Port from_port in m_input_port_list)
            {
                int from_port_index = m_input_port_list.IndexOf(from_port);
                foreach (Port to_port in from_port.GetLinkedPortList())
                {
                    Gate to_gate = to_port.GetGate();
                    if (to_gate == this)
                    {
                        int to_port_index = m_port_list.IndexOf(to_port);

                        clone_port_list[from_port_index].LinkTo(clone_port_list[to_port_index]);
                    }
                    else
                    {
                        int to_gate_index = this.GetGateList().IndexOf(to_gate);
                        int to_port_index = to_gate.GetInputPortList().IndexOf(to_port);

                        clone_port_list[from_port_index].LinkTo(clone_gate_list[to_gate_index].GetInputPort(to_port_index));
                    }
                }
            }

            foreach (Gate gate in m_gate_list)
            {
                int gate_index = m_gate_list.IndexOf(gate);

                foreach(Port from_port in gate.GetOutputPortList())
                {
                    int from_port_index = gate.GetOutputPortList().IndexOf(from_port);
                    foreach (Port to_port in from_port.GetLinkedPortList())
                    {
                        Gate to_gate = to_port.GetGate();
                        if (to_gate == this)
                        {
                            int to_port_index = m_port_list.IndexOf(to_port);

                            clone_gate_list[gate_index].GetOutputPort(from_port_index).LinkTo(clone_port_list[to_port_index]);
                        }
                        else
                        {
                            int to_gate_index = this.GetGateList().IndexOf(to_gate);
                            int to_port_index = to_gate.GetInputPortList().IndexOf(to_port);

                            clone_gate_list[gate_index].GetOutputPort(from_port_index).LinkTo(clone_gate_list[to_gate_index].GetInputPort(to_port_index));
                        }
                    }
                }
            }

            return new IC(clone_gate_list, clone_port_list, clone_port_map);
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

                    foreach(Port port in gate.GetOutputPortList())
                    {
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

    class Circuit
    {
        public List<Gate> m_gate_list;
        private Dictionary<string, Gate> m_gate_map;
        Dictionary<string, IC> m_ic_map;

        public Circuit(List<Gate> gate_list, Dictionary<string, Gate> gate_map, Dictionary<string, IC> ic_map)
        {
            m_gate_list = gate_list;
            m_gate_map = gate_map;
            m_ic_map = ic_map;
        }

        public Gate FindGateByID(string id)
        {
            return m_gate_map[id];
        }

        public List<Gate> GetGateList()
        {
            return m_gate_list;
        }

        public Gate FindICByID(string id)
        {
            return m_ic_map[id];
        }

        public static Circuit ReadXML(string xml_string)
        {
            Circuit circuit = null;

            bool in_ic = false;
            string ic_name = null;
            string ic_id = null;
            string ic_type = null;

            List<Gate> circuit_gate_list = new List<Gate>();
            bool parsed_ok = true;

            Dictionary<string, Gate> circuit_gate_map = new Dictionary<string, Gate>();
            Dictionary<string, IC> circuit_ic_map = new Dictionary<string, IC>();

            List<Gate> ic_gate_list = null;
            List<Port> ic_port_list = null;
            Dictionary<string, Gate> ic_gate_map = null;
            Dictionary<string, Port> ic_port_map = null;

            using (XmlReader reader = XmlReader.Create(new StringReader(xml_string)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("IC", StringComparison.OrdinalIgnoreCase))
                            {
                                in_ic = true;
                                ic_gate_list = new List<Gate>();
                                ic_port_list = new List<Port>();
                                ic_gate_map = new Dictionary<string, Gate>();
                                ic_port_map = new Dictionary<string, Port>();

                                if (reader.MoveToFirstAttribute())
                                {
                                    string attrib_name = reader.Name;
                                    if (attrib_name.Equals("NAME", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ic_name = reader.Value;
                                    }
                                    else if (attrib_name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ic_id = reader.Value;
                                    }
                                    else if (attrib_name.Equals("TYPE", StringComparison.OrdinalIgnoreCase))
                                    {
                                        ic_type = reader.Value;
                                    }
                                }
                            }
                            else if (reader.Name.Equals("GATE", StringComparison.OrdinalIgnoreCase))
                            {
                                string gate_name = null;
                                string gate_type = null;
                                string gate_id = null;

                                if (reader.MoveToFirstAttribute())
                                {
                                    do
                                    {
                                        string attrib_name = reader.Name;
                                        if (attrib_name.Equals("NAME", StringComparison.OrdinalIgnoreCase))
                                        {
                                            gate_name = reader.Value;
                                        }
                                        else if (attrib_name.Equals("TYPE", StringComparison.OrdinalIgnoreCase))
                                        {
                                            gate_type = reader.Value;
                                        }
                                        else if (attrib_name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                        {
                                            gate_id = reader.Value;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());

                                    if (gate_id == null)
                                    {
                                        System.Console.WriteLine(string.Format("Gate Type {0} at {1} has no ID", gate_type, ((IXmlLineInfo)reader).LineNumber));
                                    }
                                    else
                                    {
                                        Gate gate = null;

                                        if(circuit_ic_map.ContainsKey(gate_type))
                                        {
                                            gate = circuit_ic_map[gate_type].Clone();
                                        }
                                        else
                                        {
                                            gate = GenerateGate(gate_type);
                                        }

                                        if (gate == null)
                                        {
                                            System.Console.WriteLine(string.Format("Invalid Gate Type {0} at {1}", gate_type, ((IXmlLineInfo)reader).LineNumber));
                                            parsed_ok = false;
                                        }
                                        else
                                        {
                                            if (gate_name != null)
                                            {
                                                gate.SetName(gate_name);
                                            }

                                            if(in_ic)
                                            {
                                                ic_gate_map.Add(gate_id, gate);
                                            }
                                            else
                                            {
                                                circuit_gate_map.Add(gate_id, gate);
                                            }

                                            if(in_ic)
                                            {
                                                ic_gate_list.Add(gate);
                                            }
                                            else
                                            {
                                                circuit_gate_list.Add(gate);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (reader.Name.Equals("PORT", StringComparison.OrdinalIgnoreCase))
                            {
                                string port_name = null;
                                string port_direction = null;
                                string port_id = null;

                                if (reader.MoveToFirstAttribute())
                                {
                                    do
                                    {
                                        string attrib_name = reader.Name;
                                        if (attrib_name.Equals("NAME", StringComparison.OrdinalIgnoreCase))
                                        {
                                            port_name = reader.Value;
                                        }
                                        else if (attrib_name.Equals("TYPE", StringComparison.OrdinalIgnoreCase))
                                        {
                                            port_direction = reader.Value;
                                        }
                                        else if (attrib_name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                        {
                                            port_id = reader.Value;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());

                                    if (port_id == null)
                                    {
                                        System.Console.WriteLine(string.Format("Port Type {0} at {1} has no ID", port_direction, ((IXmlLineInfo)reader).LineNumber));
                                    }
                                    else
                                    {
                                        bool is_port_input = false;

                                        if (ParsePortDirection(port_direction, out is_port_input))
                                        {
                                            Port port = new Port(is_port_input);

                                            if (port_name != null)
                                            {
                                                port.SetName(port_name);
                                            }

                                            if (in_ic)
                                            {
                                                ic_port_map.Add(port_id, port);
                                                ic_port_list.Add(port);
                                            }
                                            else
                                            {
                                                System.Console.WriteLine(string.Format("Port Type {0} at {1} is not in IC element", port_direction, ((IXmlLineInfo)reader).LineNumber));
                                            }
                                        }
                                    }
                                }
                            }
                            else if (reader.Name.Equals("WIRE", StringComparison.OrdinalIgnoreCase))
                            {
                                string from_string = null;
                                string to_string = null;

                                if (reader.MoveToFirstAttribute())
                                {
                                    do
                                    {
                                        string attrib_name = reader.Name;
                                        if (attrib_name.Equals("FROM", StringComparison.OrdinalIgnoreCase))
                                        {
                                            from_string = reader.Value;
                                        }
                                        else if (attrib_name.Equals("TO", StringComparison.OrdinalIgnoreCase))
                                        {
                                            to_string = reader.Value;
                                        }
                                    }
                                    while (reader.MoveToNextAttribute());

                                    if (from_string == null || to_string == null)
                                    {
                                        if (from_string == null)
                                        {
                                            System.Console.WriteLine(string.Format("WIRE at {0} has no FROM", ((IXmlLineInfo)reader).LineNumber));
                                            parsed_ok = false;
                                        }
                                        if (to_string == null)
                                        {
                                            System.Console.WriteLine(string.Format("WIRE at {0} has no TO", ((IXmlLineInfo)reader).LineNumber));
                                            parsed_ok = false;
                                        }
                                    }
                                    else
                                    {
                                        bool from_is_port = false;
                                        string from_id = null;
                                        bool port_on_from_gate_is_input = false;
                                        int port_on_from_gate_index = -1;
                                        string port_on_from_gate_id = null;

                                        bool to_is_port = false;
                                        string to_id = null;
                                        bool port_on_to_gate_is_input = false;
                                        int port_on_to_gate_index = -1;
                                        string port_on_to_gate_id = null;

                                        bool from_parsed = ParseWireConnection(from_string, out from_is_port, out from_id, out port_on_from_gate_is_input, out port_on_from_gate_index, out port_on_from_gate_id);
                                        bool to_parsed = ParseWireConnection(to_string, out to_is_port, out to_id, out port_on_to_gate_is_input, out port_on_to_gate_index, out port_on_to_gate_id);

                                        if (!from_parsed || !to_parsed)
                                        {
                                            if (!from_parsed)
                                            {
                                                System.Console.WriteLine(string.Format("WIRE at {0} has invalid FROM", ((IXmlLineInfo)reader).LineNumber));
                                                parsed_ok = false;
                                            }

                                            if (!to_parsed)
                                            {
                                                System.Console.WriteLine(string.Format("WIRE at {0} has invalid TO", ((IXmlLineInfo)reader).LineNumber));
                                                parsed_ok = false;
                                            }
                                        }
                                        else
                                        {
                                            Port from_port = null;
                                            Port to_port = null;

                                            if (from_is_port)
                                            {
                                                if(in_ic)
                                                {
                                                    from_port = ic_port_map[from_id];
                                                }
                                                else
                                                {
                                                    System.Console.WriteLine(string.Format("WIRE at {0} has invalid FROM", ((IXmlLineInfo)reader).LineNumber));
                                                    parsed_ok = false;
                                                }
                                            }
                                            else
                                            {
                                                Gate from_gate = null;

                                                if (in_ic)
                                                {
                                                    from_gate = ic_gate_map[from_id];
                                                }
                                                else
                                                {
                                                    from_gate = circuit_gate_map[from_id];
                                                }

                                                if (from_gate != null)
                                                {
                                                    if (port_on_from_gate_index < 0)
                                                    {
                                                        from_port = from_gate.FindPortById(port_on_from_gate_id);
                                                    }
                                                    else
                                                    {
                                                        if (port_on_from_gate_is_input)
                                                        {
                                                            from_port = from_gate.GetInputPort(port_on_from_gate_index);
                                                        }
                                                        else
                                                        {
                                                            from_port = from_gate.GetOutputPort(port_on_from_gate_index);
                                                        }
                                                    }
                                                }
                                            }

                                            if (to_is_port)
                                            {
                                                if(in_ic)
                                                {
                                                    to_port = ic_port_map[to_id];
                                                }
                                                else
                                                {
                                                    System.Console.WriteLine(string.Format("WIRE at {0} has invalid TO", ((IXmlLineInfo)reader).LineNumber));
                                                    parsed_ok = false;
                                                }
                                            }
                                            else
                                            {
                                                Gate to_gate = null;

                                                if (in_ic)
                                                {
                                                    to_gate = ic_gate_map[to_id];
                                                }
                                                else
                                                {
                                                    to_gate = circuit_gate_map[to_id];
                                                }

                                                if (to_gate != null)
                                                {
                                                    if (port_on_to_gate_index < 0)
                                                    {
                                                        to_port = to_gate.FindPortById(port_on_to_gate_id);
                                                    }
                                                    else
                                                    {
                                                        if (port_on_to_gate_is_input)
                                                        {
                                                            to_port = to_gate.GetInputPort(port_on_to_gate_index);
                                                        }
                                                        else
                                                        {
                                                            to_port = to_gate.GetOutputPort(port_on_to_gate_index);
                                                        }
                                                    }
                                                }
                                            }

                                            if (from_port == null || to_port == null)
                                            {
                                                if (from_port == null)
                                                {
                                                    System.Console.WriteLine(string.Format("WIRE at {0} has invalid FROM Port", ((IXmlLineInfo)reader).LineNumber));
                                                    parsed_ok = false;
                                                }

                                                if (to_port == null)
                                                {
                                                    System.Console.WriteLine(string.Format("WIRE at {0} has invalid TO Port", ((IXmlLineInfo)reader).LineNumber));
                                                    parsed_ok = false;
                                                }
                                            }
                                            else
                                            {
                                                from_port.LinkTo(to_port);
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name.Equals("IC", StringComparison.OrdinalIgnoreCase))
                            {
                                in_ic = false;

                                IC ic = new IC(ic_gate_list, ic_port_list, ic_port_map);
                                if (ic_name != null)
                                {
                                    ic.SetName(ic_name);
                                }

                                circuit_ic_map.Add(ic_id, ic);
                            }
                            break;
                    }
                }
            }

            if (parsed_ok)
            {
                circuit = new Circuit(circuit_gate_list, circuit_gate_map, circuit_ic_map);
            }

            return circuit;
        }

        protected static Gate GenerateGate(string type)
        {
            Gate gate = null;

            if (type.Equals("AND", StringComparison.OrdinalIgnoreCase))
            {
                gate = new AndGate();
            }
            else if (type.Equals("OR", StringComparison.OrdinalIgnoreCase))
            {
                gate = new OrGate();
            }
            else if (type.Equals("NOT", StringComparison.OrdinalIgnoreCase))
            {
                gate = new NotGate();
            }
            else if (type.Equals("XOR", StringComparison.OrdinalIgnoreCase))
            {
                gate = new XorGate();
            }
            else if (type.Equals("NAND", StringComparison.OrdinalIgnoreCase))
            {
                gate = new NandGate();
            }
            else if (type.Equals("NOR", StringComparison.OrdinalIgnoreCase))
            {
                gate = new NorGate();
            }
            else if (type.Equals("BUFFER", StringComparison.OrdinalIgnoreCase))
            {
                gate = new BufferGate();
            }
            else if (type.Equals("OUTPUT", StringComparison.OrdinalIgnoreCase))
            {
                gate = new OutputOnlyGate();
            }
            else if (type.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
            {
                gate = new AlwaysTrueGate();
            }
            else if (type.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
            {
                gate = new AlwaysFalseGate();
            }
            else if (type.Equals("INPUT", StringComparison.OrdinalIgnoreCase))
            {
                gate = new InputOnlyGate();
            }
            else if (type.Equals("CLOCK", StringComparison.OrdinalIgnoreCase))
            {
                gate = new ClockGate();
            }

            return gate;
        }

        protected static bool ParsePortDirection(string s, out bool is_input)
        {
            bool parsed = false;
            is_input = false;

            if (s.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                parsed = true;
                is_input = true;
            }
            else if (s.Equals("INPUT", StringComparison.OrdinalIgnoreCase))
            {
                parsed = true;
                is_input = true;
            }
            else if (s.Equals("OUT", StringComparison.OrdinalIgnoreCase))
            {
                parsed = true;
                is_input = false;
            }
            else if (s.Equals("OUTPUT", StringComparison.OrdinalIgnoreCase))
            {
                parsed = true;
                is_input = false;
            }

            return parsed;
        }

        protected static bool ParseWireConnection(string s, out bool is_port, out string id, out bool port_on_gate_is_input, out int port_on_gate_index, out string port_on_gate_id)
        {
            bool parsed = false;
            is_port = false;
            id = null;
            port_on_gate_is_input = false;
            port_on_gate_index = -1;
            port_on_gate_id = null;

            int first_break_index = -1;
            int second_break_index = -1;

            first_break_index = s.IndexOf('$');
            second_break_index = s.IndexOf('#');

            if (first_break_index < 0)
            {
                // Port
                if (s.Length > 0)
                {
                    parsed = true;
                    is_port = true;
                    id = s;
                }
            }
            else
            {
                // Gate
                id = s.Substring(0, first_break_index);
                if (id.Length > 0)
                {
                    if(second_break_index > 0)
                    {
                        string port_on_gate_direction = s.Substring(first_break_index + 1, second_break_index - first_break_index - 1);
                        string part_on_gate_index_string = s.Substring(second_break_index + 1);

                        if (ParsePortDirection(port_on_gate_direction, out port_on_gate_is_input))
                        {
                            if (Int32.TryParse(part_on_gate_index_string, out port_on_gate_index))
                            {
                                parsed = true;
                                is_port = false;
                            }
                        }
                    }
                    else
                    {
                        port_on_gate_id = s.Substring(first_break_index + 1);
                        if (port_on_gate_id.Length > 0)
                        {
                            parsed = true;
                            is_port = false;
                        }
                    }
                }
            }

            return parsed;
        }
    }
}
