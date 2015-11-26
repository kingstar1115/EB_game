if [ ! -d ".git" ]; then
	if [ -d "ELB" ]; then
		cd ELB
		if [ -d ".git" ]; then
			git reset --hard
			git checkout master
			git fetch http://github.com/eharris93/ELB
			git merge eharris93/master
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
	git checkout master
	git fetch http://github.com/eharris93/ELB
	git merge eharris93/master
	echo "Build updated"
fi
