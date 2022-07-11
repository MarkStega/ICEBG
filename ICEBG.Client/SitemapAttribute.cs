using System;

namespace ICEBG.Client;

[AttributeUsage(AttributeTargets.Class)]
public class SitemapAttribute : Attribute
{
    public enum eChangeFreqType { always, Hourly, Daily, Weekly, Monthly, Yearly, Never };

    public eChangeFreqType changeFreq = eChangeFreqType.Monthly;
    public double priority = 0.5;

    public SitemapAttribute(eChangeFreqType changeFreq, double priority)
    {
        if (priority < 0 || priority > 1)
        {
            throw new ArgumentException($"Priority cannot be {priority} - must be between 0 and 1.");
        }

        this.changeFreq = changeFreq;
        this.priority = priority;
    }
}
