### High availability
- HA does not mean there won't be outages, it is not about preventing outages
- HA means **maximizing normal operation period (uptime)**
- Measured in nines
	- 99.9% = 8.77 hours per year downtime
	- 99.999% = 5 minutes per year downtime
- To keep high availability:
	- Have a standby server for quick replacement
	- Have a process to automatically replace a down server

### Fault tolerance
- It is stronger than HA
- It means that **it has to work through failure without disruption**
	- So there has to be redundancies in place
	- no time for downtime
	- Harder to design and implement and costs more than HA, so consider hard if you really need it
- To achieve FT:
	- Make your design connect to both the primary and standby server simultaneously, so if primary goes down, no downtime due to replacing the server with a backup

### Disaster Recovery
- What if HA and FT didn't work i.e. the system breaks below the uptime threshold?
- A Disaster Recovery plan is the plan to follow
- A DR is the procedure to **recover** **vital** systems
	- So not the whole system, focus on vital system only
	- 