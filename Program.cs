using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("Välkommen till Dynamisk Eventplanerare!\n");

        // Skapa ett event
        Event mainEvent = CreateEvent();

        // Visa tiden i flera tidszoner
        ShowEventInTimeZones(mainEvent);

        // Kontrollera konflikter med ett annat event
        CheckConflicts(mainEvent);

        // Visa tid kvar till eventet
        ShowTimeUntilEvent(mainEvent);

        // Bonus: Hantera återkommande event
        HandleRecurringEvents();
    }

    static Event CreateEvent()
    {
        Console.WriteLine("Skapa ett nytt event:");

        Console.Write("Eventnamn: ");
        string name = Console.ReadLine();

        Console.Write("Datum och tid (yyyy-MM-dd HH:mm): ");
        DateTime localTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        Console.Write("Längd (i timmar): ");
        int durationHours = int.Parse(Console.ReadLine());

        Console.Write("Längd (i minuter): ");
        int durationMinutes = int.Parse(Console.ReadLine());

        Console.Write("Tidszon (t.ex. Eastern Standard Time eller UTC-offset som +05:30): ");
        string timeZoneInput = Console.ReadLine();
        TimeZoneInfo timeZone = ParseTimeZone(timeZoneInput);

        Event newEvent = new Event(name, localTime, durationHours, durationMinutes, timeZone);

        Console.WriteLine("Event skapades framgångsrikt!\n");
        return newEvent;
    }

    static void ShowEventInTimeZones(Event ev)
    {
        Console.WriteLine("Visa eventet i flera tidszoner:");

        List<string> selectedTimeZones = new List<string>();
        for (int i = 1; i <= 3; i++)
        {
            Console.Write($"Tidszon {i}: ");
            selectedTimeZones.Add(Console.ReadLine());
        }

        Console.WriteLine($"\nTider för eventet \"{ev.Name}\":");
        foreach (string tz in selectedTimeZones)
        {
            try
            {
                TimeZoneInfo selectedZone = ParseTimeZone(tz);
                DateTime convertedTime = TimeZoneInfo.ConvertTime(ev.StartTime, ev.EventTimeZone, selectedZone);
                Console.WriteLine($"{selectedZone.DisplayName}: {convertedTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid konvertering av tidszon: {ex.Message}");
            }
        }
    }

    static void CheckConflicts(Event ev)
    {
        Console.WriteLine("\nKontrollera konflikt med ett annat event:");

        Event otherEvent = CreateEvent();

        DateTime evEndTime = ev.StartTime.Add(ev.Duration);
        DateTime otherEndTime = otherEvent.StartTime.Add(otherEvent.Duration);

        if ((ev.StartTime < otherEndTime && evEndTime > otherEvent.StartTime))
        {
            Console.WriteLine("Eventen överlappar!");
        }
        else
        {
            Console.WriteLine("Eventen överlappar inte.");
        }
    }

    static void ShowTimeUntilEvent(Event ev)
    {
        TimeSpan timeUntilEvent = ev.StartTime - DateTime.Now;

        if (timeUntilEvent.TotalSeconds > 0)
        {
            Console.WriteLine("\nTid kvar till eventet:");
            Console.WriteLine($"{timeUntilEvent.Days} dagar, {timeUntilEvent.Hours} timmar, {timeUntilEvent.Minutes} minuter, {timeUntilEvent.Seconds} sekunder\n");
        }
        else
        {
            Console.WriteLine("Eventet har redan inträffat.\n");
        }
    }

    static void HandleRecurringEvents()
    {
        Console.WriteLine("\nHantera återkommande event:");

        Console.Write("Eventnamn: ");
        string name = Console.ReadLine();

        Console.Write("Dag i veckan (t.ex. Måndag): ");
        DayOfWeek recurringDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), Console.ReadLine(), true);

        Console.Write("Tid (HH:mm): ");
        TimeSpan recurringTime = TimeSpan.Parse(Console.ReadLine());

        Console.Write("Antal tillfällen: ");
        int occurrences = int.Parse(Console.ReadLine());

        DateTime now = DateTime.Now;
        List<DateTime> upcomingOccurrences = new List<DateTime>();

        for (int i = 0; upcomingOccurrences.Count < occurrences; i++)
        {
            DateTime nextOccurrence = now.AddDays(i);
            if (nextOccurrence.DayOfWeek == recurringDay && nextOccurrence.TimeOfDay <= recurringTime)
            {
                upcomingOccurrences.Add(nextOccurrence.Date + recurringTime);
            }
        }

        Console.WriteLine("Kommande tillfällen:");
        foreach (DateTime occurrence in upcomingOccurrences)
        {
            Console.WriteLine(occurrence);
        }
    }

    static TimeZoneInfo ParseTimeZone(string timeZoneInput)
    {
        if (timeZoneInput.StartsWith("UTC") || timeZoneInput.StartsWith("+"))
        {
            return TimeZoneInfo.CreateCustomTimeZone("Custom", TimeSpan.Parse(timeZoneInput), "Custom", "Custom");
        }
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneInput);
    }
}

class Event
{
    public string Name { get; }
    public DateTime StartTime { get; }
    public TimeSpan Duration { get; }
    public TimeZoneInfo EventTimeZone { get; }

    public Event(string name, DateTime startTime, int durationHours, int durationMinutes, TimeZoneInfo timeZone)
    {
        Name = name;
        StartTime = startTime;
        Duration = new TimeSpan(durationHours, durationMinutes, 0);
        EventTimeZone = timeZone;
    }
}
