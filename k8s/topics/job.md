
 It is like deployment, but it creates pods that carry out batch tasks i.e. a pod that has an ending state (since usually pods are always alive e.g. a webserver). So, in vfi, the new orchestrator should create a Job object, which is then the analog of our Jenkins master creating worker pod. 

 Example to create a job that calculates pi:
 
 `kubectl create job calculatepi --image=perl:5.34.0 -- "perl" "-Mbignum=bpi" "-wle" "print bpi(2000)"`
 
 When we watch the job `watch kubectl get jobs`, when complete we will get output like

 ```
 NAME          COMPLETIONS   DURATION   AGE
calculatepi   1/1           32s        2m6s
 ```

 Note that the pod will have the job name appended to the front 
 ```
kubectl get pods
NAME                READY   STATUS      RESTARTS   AGE
calculatepi-v2wc4   0/1     Completed   0          27m
 ```

 Thus the stdout of the job can be gotten with the trick like

 ```
kubectl logs $(kubectl get pods | grep calculatepi | awk {'print $1'})

3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679821480865132823066470938446095505822317253594081284811174502841027019385211055596446229489549303819644288109756659334461284756482337867831652712019091456485669234603486104543266482133936072602491412737245870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330572703657595919530921861173819326117931051185480744623799627495673518857527248912279381830119491298336733624406566430860213949463952247371907021798609437027705392171762931767523846748184676694051320005681271452635608277857713427577896091736371787214684409012249534301465495853710507922796892589235420199561121290219608640344181598136297747713099605187072113499999983729780499510597317328160963185950244594553469083026425223082533446850352619311881710100031378387528865875332083814206171776691473035982534904287554687311595628638823537875937519577818577805321712268066130019278766111959092164201989380952572010654858632788659361533818279682303019520353018529689957736225994138912497217752834791315155748572424541506959508295331168617278558890750983817546374649393192550604009277016711390098488240128583616035637076601047101819429555961989467678374494482553797747268471040475346462080466842590694912933136770289891521047521620569660240580381501935112533824300355876402474964732639141992726042699227967823547816360093417216412199245863150302861829745557067498385054945885869269956909272107975093029553211653449872027559602364806654991198818347977535663698074265425278625518184175746728909777727938000816470600161452491921732172147723501414419735685481613611573525521334757418494684385233239073941433345477624168625189835694855620992192221842725502542568876717904946016534668049886272327917860857843838279679766814541009538837863609506800642251252051173929848960841284886269456042419652850222106611863067442786220391949450471237137869609563643719172874677646575739624138908658326459958133904780275901
 ```
 
 More info on jobs and its possibilities `kubectl explain job.spec | more`

 Important to note here is the option completions and parallelism
 ```
 completions   <integer>
    Specifies the desired number of successfully finished pods the job should be
    run with.  Setting to null means that the success of any pod signals the
    success of all pods, and allows parallelism to have any positive value.
    Setting to 1 means that parallelism is limited to 1 and the success of that
    pod signals the success of the job. More info:
    https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/

parallelism   <integer>
    Specifies the maximum desired number of pods the job should run at any given
    time. The actual number of pods running in steady state will be less than
    this number when ((.spec.completions - .status.successful) <
    .spec.parallelism), i.e. when the work left to do is less than max
    parallelism. More info:
    https://kubernetes.io/docs/concepts/workloads/controllers/jobs-run-to-completion/
  ```

  With this options, a deployment manifest like
  ```
apiVersion: batch/v1
kind: Job
metadata:
  creationTimestamp: null
  name: calculatepi
spec:
  completions: 20
  parallelism: 5
  template:
    metadata:
      creationTimestamp: null
    spec:
      containers:
      - command:
        - perl
        - -Mbignum=bpi
        - -wle
        - print bpi(2000)
        image: perl:5.34.0
        name: calculatepi
        resources: {}
      restartPolicy: Never
  ```

 This will generate, by the end of job execution, 20 pods that will do the command, and at any given time, only 5 pods can exist.

 Notice above that when we did `kubectl get jobs` we get cluttered with history of completed jobs. We can delete a job like `kubectl delete job/calculatepi`. But what if we want to keep only a certain number of completed jobs to retain? See CronJob below!
 
 ## CronJob
 CronJob resources will Job resource at approximately the scheduled time. The Job then creates the worker pods. The syntax is very similar.
 
 ```
apiVersion: batch/v1
kind: CronJob
metadata:
  creationTimestamp: null
  name: calculatepi
spec:
  completions: 20
  parallelism: 5
  template:
    metadata:
      creationTimestamp: null
    spec:
      containers:
      - command:
        - perl
        - -Mbignum=bpi
        - -wle
        - print bpi(2000)
        image: perl:5.34.0
        name: calculatepi
        resources: {}
      restartPolicy: Never
  successfulJobsHistoryLimit: 3
 ```

 The above will:
 - Create a job every minute, where each job will eventually create 20 pods by the end, with only 5 pods can run at the same time.
 - Each pod will calculate pi
 - It will keep only 3 successful job history; this is handy to keep the history of job executions clean!
 - The history may surge to 4 for a little bit, but it will go back to 3 eventually

 It may happen that the Job or pod is created and run relatively late. You may have a hard requirement for the job to not be started too far over the scheduled time. In that case, you can specify a deadline by specifying the startingDeadlineSeconds field in the CronJob specification.

 In normal circumstances, a CronJob always creates only a single Job for each execution configured in the schedule, but it may happen that two Jobs are created at the same time, or none at all. To combat the first problem, your jobs should be idempotent (running them multiple times instead of once shouldn’t lead to unwanted results). For the second problem, make sure that the next job run performs any work that should have been done by the previous (missed) run.

 Thus, CronJob are for tasks that:
 - Don't have a problem being run multiple times; since a delay  of the first execution could make the second execution execute during the first execution! So, your command needs to have good error handling (e.g. if it writes something to db, perhaps do like `get_or_create` of Django)
 - A job may even not execute at all (missed) due to some reason! Thus, the command must be such that it is 'complete' i.e. does not rely on any previous execution e.g. not relying on the time of the previous execution to do some command, since the previous execution may never even happen!
