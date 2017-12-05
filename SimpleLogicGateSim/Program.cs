using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimpleLogicGateSim
{
    class Program
    {
        static void Main(string[] args)
        {
#if false
            Gate output1 = new OutputOnlyGate();
            Gate output2 = new OutputOnlyGate();

            // Gate logic_gate = new AndGate();
            Gate logic_gate = new OrGate();
            // Gate logic_gate = new XorGate();

            Gate input1 = new InputOnlyGate();

            List<Gate> gate_list = new List<Gate>();
            gate_list.Add(output1);
            gate_list.Add(output2);
            gate_list.Add(logic_gate);
            gate_list.Add(input1);

            output1.GetOutputPort(0).LinkTo(logic_gate.GetInputPort(0));
            output2.GetOutputPort(0).LinkTo(logic_gate.GetInputPort(1));
            logic_gate.GetOutputPort(0).LinkTo(input1.GetInputPort(0));

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

#endif

#if false

            Gate output3 = new OutputOnlyGate();
            Gate not_gate = new NotGate();
            Gate input2 = new InputOnlyGate();
            List<Gate> gate_list = new List<Gate>();
            gate_list.Add(output3);
            gate_list.Add(not_gate);
            gate_list.Add(input2);

            output3.GetOutputPort(0).LinkTo(not_gate.GetInputPort(0));
            not_gate.GetOutputPort(0).LinkTo(input2.GetInputPort(0));

            output3.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input2.GetInputPort(0).GetState());

            output3.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input2.GetInputPort(0).GetState());
#endif

#if false
            Gate clock = new Clock();
            Gate input = new InputOnlyGate();
            List<Gate> gate_list = new List<Gate>();
            gate_list.Add(clock);
            gate_list.Add(input);

            clock.GetOutputPort(0).LinkTo(input.GetInputPort(0));

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());

            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input.GetInputPort(0).GetState());
#endif

#if false
            // Gate lg1 = new AndGate();
            // Gate lg2 = new AndGate();

            // Gate lg1 = new OrGate();
            // Gate lg2 = new OrGate();

            // Gate lg1 = new AndGate();
            // Gate lg2 = new OrGate();

            Gate lg1 = new OrGate();
            Gate lg2 = new AndGate();

            lg1.GetOutputPort(0).LinkTo(lg2.GetInputPort(0));

            Port icport1 = new Port(true);
            Port icport2 = new Port(true);
            Port icport3 = new Port(true);
            Port icport4 = new Port(false);

            icport1.LinkTo(lg1.GetInputPort(0));
            icport2.LinkTo(lg1.GetInputPort(1));
            icport3.LinkTo(lg2.GetInputPort(1));
            lg2.GetOutputPort(0).LinkTo(icport4);

            List<Gate> ic_gate_list = new List<Gate>();
            ic_gate_list.Add(lg1);
            ic_gate_list.Add(lg2);

            List<Port> ic_port_list = new List<Port>();
            ic_port_list.Add(icport1);
            ic_port_list.Add(icport2);
            ic_port_list.Add(icport3);
            ic_port_list.Add(icport4);

            IC ic1 = new IC(ic_gate_list, ic_port_list);

            Gate output1 = new OutputOnlyGate();
            Gate output2 = new OutputOnlyGate();
            Gate output3 = new OutputOnlyGate();

            Gate input1 = new InputOnlyGate();

            output1.GetOutputPort(0).LinkTo(ic1.GetInputPort(0));
            output2.GetOutputPort(0).LinkTo(ic1.GetInputPort(1));
            output3.GetOutputPort(0).LinkTo(ic1.GetInputPort(2));
            ic1.GetOutputPort(0).LinkTo(input1.GetInputPort(0));

            List<Gate> gate_list = new List<Gate>();
            gate_list.Add(ic1);
            gate_list.Add(output1);
            gate_list.Add(output2);
            gate_list.Add(output3);
            gate_list.Add(input1);

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(false);
            output3.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(false);
            output3.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(true);
            output3.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(true);
            output3.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(false);
            output3.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(false);
            output3.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(true);
            output3.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(true);
            output3.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState());
#endif

#if false
            // string cwd = Directory.GetCurrentDirectory();
            // System.Console.WriteLine(cwd);
            string text = System.IO.File.ReadAllText(@"..\..\..\ic\half_adder.xml");
            IC ic2 = ICFactory.ReadXML(text);

            IC ic1 = ic2.Clone() as IC;

            Gate output1 = new OutputOnlyGate();
            Gate output2 = new OutputOnlyGate();
            Gate input1 = new InputOnlyGate();
            Gate input2 = new InputOnlyGate();

            output1.GetOutputPort(0).LinkTo(ic1.GetInputPort(0));
            output2.GetOutputPort(0).LinkTo(ic1.GetInputPort(1));
            ic1.GetOutputPort(0).LinkTo(input1.GetInputPort(0));
            ic1.GetOutputPort(1).LinkTo(input2.GetInputPort(0));

            List<Gate> gate_list = new List<Gate>();
            gate_list.Add(ic1);
            gate_list.Add(output1);
            gate_list.Add(output2);
            gate_list.Add(input1);
            gate_list.Add(input2);

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());
#endif

#if true
            // string cwd = Directory.GetCurrentDirectory();
            // System.Console.WriteLine(cwd);
            string text = System.IO.File.ReadAllText(@"..\..\..\ic\circuit2.xml");
            Circuit circuit1 = Circuit.ReadXML(text);

            List<Gate> gate_list = circuit1.GetGateList();
            Gate output1 = circuit1.FindGateByID("out1");
            Gate output2 = circuit1.FindGateByID("out2");
            Gate input1 = circuit1.FindGateByID("in1");
            Gate input2 = circuit1.FindGateByID("in2");

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(false);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(false);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());

            output1.GetOutputPort(0).SetState(true);
            output2.GetOutputPort(0).SetState(true);
            LogicSimulator.RunTick(gate_list);
            System.Console.WriteLine(input1.GetInputPort(0).GetState() + "," + input2.GetInputPort(0).GetState());
#endif
        }
    }
}
