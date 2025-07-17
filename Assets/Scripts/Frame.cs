using System;
using System.Collections.Generic;

[Serializable]
public class Frame
{
    public string id;
    public string type;
    public string nextId;

    [NonSerialized]
    public FrameType frameType;

    public string characterName;
    public string characterPosition;
    public string spritePath;
    public string text;
    public List<Option> options;
}

[Serializable]
public class Option
{
    public string title;
    public string nextFrameId;
}

[Serializable]
public class FrameList
{
    public List<Frame> frames;
}

public enum FrameType { Dialog, Text, Choice, Final }