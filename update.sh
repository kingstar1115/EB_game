if [ ! -d ".git" ]; then
	if [ -d "ELB" ]; then
		cd ELB
		if [ -d ".git" ]; then
			git reset --hard
			git clean -fd
			git checkout master	
			git remote add ewan http://github.com/eharris93/ELB 
			git fetch ewan
			git merge ewan/master
			echo "Build updated"
		else
			cd..
			rm -rf ELB
			git clone http://github.com/eharris93/ELB
			echo "New build downloaded"
		fi
	else
		git clone http://github.com/eharris93/ELB
		echo "New build downloaded"
	fi
elif [ -d ".git" ]; then
	git reset --hard
	git clean -fd
	git checkout master	
	git remote add ewan http://github.com/eharris93/ELB 
	git fetch ewan
	git merge ewan/master
	echo "Build updated"
fi
