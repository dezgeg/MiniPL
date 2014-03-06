var i : int;
// an error in the for statement "header" should not cause the loop to be missed
for i in 1000 asdasd sasdsd do
        print i; // must not complain
	print bar; // must complain
end for; // this must not complain about unexpected 'end for'
