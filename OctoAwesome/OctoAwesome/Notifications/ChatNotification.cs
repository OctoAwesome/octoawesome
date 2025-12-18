using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications;

[Nooson, SerializationId()]
public partial class ChatNotification : SerializableNotification, IConstructionSerializable<ChatNotification>
{
    public string Text { get; set; }
    public string Username { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

    protected override void OnRelease()
    {
        Text = "";
        Username = "";
        TimeStamp = DateTimeOffset.MinValue;

        base.OnRelease();
    }
}
