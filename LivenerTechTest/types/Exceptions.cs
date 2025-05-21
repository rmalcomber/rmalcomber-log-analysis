namespace LivenerTechTest.types;

public class FailedToParseLineException(string line) : Exception($"Failed to parse line: {line}");