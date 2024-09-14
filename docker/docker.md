# Docker

## General stuff
- Containers are instances of an image
-  Get all running containers and images inside it: ```sudo docker ps -a```
- Get into failed docker build: ```sudo docker run -i -t (last successfully built image inside the container where the build failed) sh```
- Delete all containers: ```docker container prune```
- Delete one container: ```docker rm CONTAINER```
- Delete all images and containers: ```docker system prune --all"```
- Delete one image: ```docker rmi IMAGE```
- MYSQL image draai niet; exit met 137 error. Oplossing: zie https://unix.stackexchange.com/questions/128642/debug-out-of-memory-with-var-log-messages. In kortom: Linux kan mischieen je container afmaken zonder je het te weten want je heeft geen genoeg RAM :(
- Kan geen verbinding maken met een MYSQL container met een shared volume, hoewel je de environment variabelen hebben toegepast in Docker Compose, en toegang van buiten hebben toelaten. Oplossing: https://github.com/docker-library/mysql/issues/275#issuecomment-292243855. In kortom: In de eerste poging van `docker-compose up`, zorg ervoor dat de volume die in `volume` rij van je `docker-compose.yml` NIET BESTAAT!!! Anders, wordt de environment variabelen niet toegepast!
- Make non-root user (who is sudoer) able to run docker without sudo: `sudo usermod -aG docker $USER`
- Shell vs. Exec form of CMD and Entrypoint; it matters: https://emmer.dev/blog/docker-shell-vs.-exec-form/#signal-trapping-forwarding (use internet archive if the link is gone)

## Theory

- What is the difference between VM and Containers?
  - VM virtualises hardware (`hypervisor virtualization`). 
    - This means it takes physical servers and slices them into virtual servers that we call virtual machines (VM). 
    - For example, CPUs are sliced into virtual CPUs, storage is sliced into virtual storage, and network cards are sliced into virtual network cards. These virtual resources are combined into VMs that look, smell, and feel like regular physical servers. 
    - You then install a single OS and application in each VM. 
  - Container virtualizes operating systems
    - It takes operating systems like Linux and Windows and slices them into virtual operating systems called containers. 
    - For example, process trees are sliced into virtual process trees, filesystems are sliced into virtual filesystems, and network interfaces are sliced into virtual interfaces. 
    - These virtual resources are combined into containers that look, smell, and feel like regular operating systems. 
    - You then run a single application in each container. 
  - Diagram:
  ![](vmvscontainer.png)
  - As seen above, if you slice up VMs, you run a new OS on each. If you slice up the OS, you run each app inside each OS slice.
  - Containers are more lighter weight than hypervisor virtualization (i.e. VMs)
- Every node that wants to run containers need a `container runtime`
  - A runtime can be either `high level` or `low level`
  - A `low level` container directly runs containers
    - Example: `runc`, `crun`, `kata-runtime`
  - A `high level` container manages containers and its images
    - Example: `containerd`, `rkt`, `Docker`, `Podman`
    - They talk to the `runc` low level `container runtime` to run the containers
    - Some are limited in capability (e.g. `containerd` cannot build images), while others are more powerful but more bloated (e.g. `Docker` can build images)
    - However, some depend on each other to work (e.g. `Docker` needs specifically `containerd` to talk to `runc`; it cannot talk to `runc` directly)
  - There is a standard called `OCI` (Open Container Initiative) that defines standards on `images` and `runtimes`
    - OCI compliant images are images that are guaranteed to be able to be run by OCI compliant runtimes
    - Tools that can build `OCI compliant images` include `Docker` or `Buildah`
    - Container runtimes such as `runc` and `kata` are guaranteed to be able to run these images; thus they are an `OCI compliant runtime`.
    - Thus, `OCI compliant runtime` term is only really applicable to `low-level` container runtimes
  Put image here
- To orchestrate containers, we can use Kubernetes
  - Look at k8s notes first to understand this section
  - To orchestrate containers, the `kubelet` needs to communicate with a high level container runtime, and, this high level container runtime has to implement the `CRI` (`Container Runtime Interface`)
  - So, in the ideal world, we need a chain of high and low level container runtimes, where the chain:
    - Speaks `CRI` at its input side so kubelet can talk to it
    - OCI compliant so it can run standard images
    - Most high-level container runtimes need a translator (i.e. `shim`) to translate the `CRI` so that kubelet can talk to it
    - Below image explains many possible alternatives