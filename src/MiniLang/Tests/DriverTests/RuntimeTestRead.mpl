print "Testing read statement... ";

var i : int;
var s : string;
var b : bool;

read i;
assert i = 42;
read i;
assert i = -100;

read s;
assert s = "asd";
read s;
assert s = "";

read b;
assert b;
read b;
assert !b;

print "OK so far\n";

var not_in_file : int;
read not_in_file;

// not reached
print "FAIL!\n";
assert false;