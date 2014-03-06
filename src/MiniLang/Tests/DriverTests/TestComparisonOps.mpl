print "Testing comparison operators.\n";

assert false < true;
assert false <= true;
assert true > false;
assert true >= false;

assert true = true;
assert false != true;
assert !(true < true);
assert !(false > false);

assert 1 < 2;
assert 1 <= 2;
assert 2 > 1;
assert 2 >= 1;
assert 1 = 1;
assert 1 != 2;
assert !(1 < 1);
assert !(2 > 2);

assert "bar" < "foo";
assert "foo" != "bar";
assert "baz\n" = "baz\n";

print "OK\n";