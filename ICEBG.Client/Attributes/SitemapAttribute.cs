using System;

namespace ICEBG.Client;

/// <summary>
/// Indicates that a razor page should be added to the sitemap. Priority must be within the range zero to one.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SitemapAttribute : Attribute
{
    /// <summary>
    /// Indicates the change frequency to be shown in the sitemap.
    /// </summary>
    public enum eChangeFreqType { Always, Hourly, Daily, Weekly, Monthly, Yearly, Never };


    /// <summary>
    /// The user selected change frequency.
    /// </summary>
    public readonly eChangeFreqType ChangeFreq;
    
    
    /// <summary>
    /// The user selected priority.
    /// </summary>
    public double Priority;


    public SitemapAttribute(eChangeFreqType changeFreq, double priority)
    {
        if (priority < 0 || priority > 1)
        {
            throw new ArgumentException($"Priority cannot be {priority} - must be between 0 and 1.");
        }

        ChangeFreq = changeFreq;
        Priority = priority;
    }
}
