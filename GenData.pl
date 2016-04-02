#!/usr/bin/perl

open(DATAFILE, 'processes.txt');

$num_args = $#ARGV + 1;
if($num_args != 3) {
	print "\nWrong number of arguments passed\n";
	print "There should be 3 arguments: Number of processes, MAX CPU burst time, and MAX I/O burst time";
	exit;
}

$num_processes = $ARGV[0];
$max_cpu_burst = $ARGV[1];
$max_io_burst = $ARGV[2];

$process_id = 0;

my $filename = "processDataFile.txt";

unless(open FILE, '>'.$filename) {
	die "\nUnable to open file\n";
}

$previousArrivalTime = 0;

if($num_processes > 0 && $max_cpu_burst > 0 && $max_io_burst > 0) {
	for($i = 0; $i < $num_processes; $i++) {
		$line = "$process_id ";
		$process_id += 1;

		$arrivalTime = int(rand(10)) + 1;
		$arrivalTime += $previousArrivalTime;
		$previousArrivalTime = $arrivalTime;

		$line .= "$arrivalTime ";

		$num_bursts = int(rand(5)) + 1;

		$total_bursts = 2 * $num_bursts + 1;

		$line .= "$total_bursts";

		for($j = 0; $j < $num_bursts; $j++) {
			$cpu_burst = int(rand($max_cpu_burst)) + 1;
			$io_burst = int(rand($max_io_burst)) + 1;
			$line .= " $cpu_burst ";
			$line .= "$io_burst";
		}

		$cpu_burst = int(rand($max_cpu_burst)) + 1;
		$line.= " $cpu_burst";

		print FILE "$line\r\n";
	}
} else {
	print "Invalid arguments passed!\n";
	close FILE;
	exit;
}

close FILE;

