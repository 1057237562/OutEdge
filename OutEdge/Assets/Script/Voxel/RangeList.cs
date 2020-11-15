using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range
{

    public Range(int s,int c)
    {
        start = s;
        count = c;
    }

    public Range() { }

    public int start;
    public int count = 0;
}

public class RangeList : Range
{
    private List<Range> ranges;

    //public int delta = 0;

    public RangeList(Range[] r)
    {
        ranges = new List<Range>(r);
    }

    public RangeList()
    {
        ranges = new List<Range>();
    }

    public void Add(Range r) {
        r.start = count;
        count += r.count;
        //delta += r.count;

        ranges.Add(r);
    }

    public int GetRangeCount()
    {
        return ranges.Count;
    }

    public void Remove(int index)
    {
        ShiftRange(index, -ranges[index].count);

        count -= ranges[index].count;
        //delta -= ranges[index].count;

        ranges.RemoveAt(index);
    }

    public void Replace(int index,Range replacement)
    {
        ShiftRange(index + 1, replacement.count - ranges[index].count);

        count += replacement.count - ranges[index].count;
        //delta += replacement.count - ranges[index].count;

        ranges[index] = replacement;
    }

    public void Update(int index, int basis)
    {
        ShiftRange(index + 1, basis);

        count += basis;
        //delta += replacement.count - ranges[index].count;
    }

    /*public void Replace(int index, RangeList replacement)
    {
        ShiftRange(index + 1, replacement.delta);

        count += replacement.delta;
        delta += replacement.delta;
        replacement.delta = 0;

        ranges[index] = replacement;
    }*/

    public Range GetRange(int index)
    {
        return ranges[index];
    }

    private void ShiftRange(int startIndex,int offset)
    {
        for(int i = startIndex; i < ranges.Count; i++)
        {
            ranges[i].start += offset;
        }
    }
}
