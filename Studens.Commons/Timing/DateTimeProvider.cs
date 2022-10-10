﻿namespace Studens.Commons;

/// <summary>
/// Default implementation for <see cref="IDateTimeProvider"/>
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
	public bool TimeZoneExists(string timeZoneId) =>
	TimeZoneInfo.FindSystemTimeZoneById(timeZoneId) is not null;

	/// <summary>
	/// Returns current date time for <paramref name="timeZoneId"/>.
	/// If timezone id is not valid, default timezone will be used.
	/// </summary>
	/// <param name="timeZoneId"></param>
	/// <returns></returns>
	public DateTime GetByTimeZone(string timeZoneId)
	{
		//TODO: tryc this and return default/throw
		var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
		var utcNow = DateTime.UtcNow;

		return TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
	}

	public DateTime ConvertToUtc(DateTime dateTime, string timeZoneId)
	{
		if (dateTime.Kind == DateTimeKind.Utc)
		{
			return dateTime;
		}

		var timeZone = GetTimeZoneByIdOrDefault(timeZoneId);

		if (timeZone.IsInvalidTime(dateTime))
		{
			return dateTime;
		}

		return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
	}

	public DateTime ConvertToLocal(DateTime dateTime, string? timeZoneId)
	{
		if (dateTime.Kind == DateTimeKind.Local)
		{
			return dateTime;
		}

		var timeZone = GetTimeZoneByIdOrDefault(timeZoneId);

		if (timeZone.IsInvalidTime(dateTime))
		{
			return dateTime;
		}

		return TimeZoneInfo.ConvertTime(dateTime, timeZone);
	}

	public bool IsInvalidTime(DateTime dateTime, string timeZoneId)
	{
		var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

		return timeZone.IsInvalidTime(dateTime);
	}

	public bool IsAmbiguousTime(DateTime dateTime, string timeZoneId)
	{
		var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

		return timeZone.IsAmbiguousTime(dateTime);
	}

	private TimeZoneInfo GetTimeZoneByIdOrDefault(string timeZoneId) =>
	TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
}