using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer;
internal static class Extensions
{

    internal static Command Create(this Command command, string name, string description)
    {
        var childCommand = new Command(name, description);

        command.Add(childCommand);
        return childCommand;
    }
    internal static Command Create(this Command command, string name, string description, Action handler)
    {
        var childCommand = new Command(name, description);

        childCommand.SetHandler(handler);
        command.Add(childCommand);
        return childCommand;
    }

    internal static Command Create<TArg>(this Command command, string name, string description, Action<TArg> handler, Option<TArg> option)
    {
        var childCommand = new Command(name, description)
        {
            option
        };

        childCommand.SetHandler(handler, option);
        command.Add(childCommand);
        return childCommand;
    }
    internal static Command Create<TArg1, TArg2>(this Command command, string name, string description, Action<TArg1, TArg2> handler, Option<TArg1> option1, Option<TArg2> option2)
    {
        var childCommand = new Command(name, description)
        {
            option1,
            option2
        };

        childCommand.SetHandler(handler, option1, option2);
        command.Add(childCommand);
        return childCommand;
    }
    internal static Command Create<TArg1, TArg2, TArg3>(this Command command,string name, string description, Action<TArg1, TArg2, TArg3> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3)
    {
        var childCommand = new Command(name, description)
        {
            option1,
            option2,
            option3
        };

        childCommand.SetHandler(handler, option1, option2, option3);
        command.Add(childCommand);
        return childCommand;
    }
    internal static Command Create<TArg1, TArg2, TArg3, TArg4>(this Command command,string name, string description, Action<TArg1, TArg2, TArg3, TArg4> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3, Option<TArg4> option4)
    {
        var childCommand = new Command(name, description)
        {
            option1,
            option2,
            option3,
            option4
        };

        childCommand.SetHandler(handler, option1, option2, option3, option4);
        command.Add(childCommand);
        return childCommand;
    }
    internal static Command Create<TArg1, TArg2, TArg3, TArg4, TArg5>(this Command command,string name, string description, Action<TArg1, TArg2, TArg3, TArg4, TArg5> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3, Option<TArg4> option4, Option<TArg5> option5)
    {
        var childCommand = new Command(name, description)
        {
            option1,
            option2,
            option3,
            option4,
            option5
        };

        childCommand.SetHandler(handler, option1, option2, option3, option4, option5);
        command.Add(childCommand);
        return childCommand;
    }

    internal static Command Add<TArg>(this Command parent, string name, string description, Action<TArg> handler, Option<TArg> option)
    {
        parent.Create(name, description, handler, option);
        return parent;
    }
    internal static Command Add<TArg1, TArg2>(this Command parent, string name, string description, Action<TArg1, TArg2> handler, Option<TArg1> option1, Option<TArg2> option2)
    {
        parent.Create(name, description, handler, option1, option2);
        return parent;
    }
    internal static Command Add<TArg1, TArg2, TArg3>(this Command parent, string name, string description, Action<TArg1, TArg2, TArg3> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3)
    {
        parent.Create(name, description, handler, option1, option2, option3);
        return parent;
    }
    internal static Command Add<TArg1, TArg2, TArg3, TArg4>(this Command parent, string name, string description, Action<TArg1, TArg2, TArg3, TArg4> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3, Option<TArg4> option4)
    {
        parent.Create(name, description, handler, option1, option2, option3, option4);
        return parent;
    }
    internal static Command Add<TArg1, TArg2, TArg3, TArg4, TArg5>(this Command parent, string name, string description, Action<TArg1, TArg2, TArg3, TArg4, TArg5> handler, Option<TArg1> option1, Option<TArg2> option2, Option<TArg3> option3, Option<TArg4> option4, Option<TArg5> option5)
    {
        parent.Create(name, description, handler, option1, option2, option3, option4, option5);
        return parent;
    }
}
