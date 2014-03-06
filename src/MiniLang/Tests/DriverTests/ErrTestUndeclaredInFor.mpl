// an undeclared loop iterator should cause the iterator variable to be declared
for undeclared in 1..10 do // should complain about 'undeclared' not declared
        print undeclared + 1; // should not complain
	print bar; // should complain
end for;