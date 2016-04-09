// main
package main

import (
	"bufio"
	"bytes"
	"fmt"
	"io"
	"io/ioutil"
	"net/http"
	"os"
	"os/exec"
	"math/rand"
	"time"
)

const letterBytes = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"

func RandStringBytes(n int) string {
    b := make([]byte, n)
    for i := range b {
        b[i] = letterBytes[rand.Intn(len(letterBytes))]
    }
    return string(b)
}

// Exists reports whether the named file or directory exists.
func Exists(name string) bool {
    if _, err := os.Stat(name); err != nil {
    if os.IsNotExist(err) {
                return false
            }
    }
    return true
}

func saveFile(r *http.Request, name string) (string, error) {
	file, header, err := r.FormFile(name)
	if err != nil {
		return "", err
	}

	//  ext := strings.ToLower(header.Header.Get("Content-Type"))
	filename := "/home/jannek/serv/" + header.Filename

	//	if ext == "image/png" {
	//		filename = name + ".png"
	//	} else if ext == "image/jpg" || ext == "image/jpeg" {
	//		filename = name + ".jpg"
	//	} else {
	//		err = fmt.Errorf("Wrong mime type " + ext)
	//		return "", err
	//	}

	f, err := os.Create(filename)

	w := bufio.NewWriter(f)
	io.Copy(w, file)

	f.Close()

	return filename, nil
}

func processImage(cName string, sName string, gpu string, outName string) {
	fmt.Println(cName + " ready to be processed with " + sName)



	os.Chdir("/home/jannek/neural-style/")

	//	cmdString := "/home/jannek/neural-style/neural_style.lua" +
	//		" -style_image examples/inputs/starry_night.jpg" +
	//		" -content_image examples/inputs/brad_pitt.jpg" +
	//		" -image_size 512 -gpu 2"

	//	fmt.Println("th " + cmdString)

	cmd := exec.Command("th", "/home/jannek/neural-style/neural_style.lua",
		"-style_image", sName,
		"-content_image", cName,
		"-output_image", "/home/jannek/serv2/output" + outName + ".png",
		"-save_iter", "100",
		"-print_iter", "0",
		"-image_size", "512",
		"-gpu", gpu)
	//	cmd.Dir = "/home/jannek/neural-style/"

	fmt.Println(cmd.Path)

	// "neural_style.lua",
	//		"-style_image", "examples/inputs/starry_night.jpg",
	//		"-content_image", "examples/inputs/brad_pitt.jpg",
	//		"-output_image", "output.png", "-image_size", "256", "-gpu", "2"

	var out bytes.Buffer
	var stderr bytes.Buffer
	cmd.Stdout = &out
	cmd.Stderr = &stderr

	err := cmd.Run()
	cmd.Process.Kill()

	if err != nil {
		fmt.Println("Command not executed!")
		fmt.Println(fmt.Sprint(err) + ": " + stderr.String())
		return
	}
}

func serve(w http.ResponseWriter, r *http.Request) {
	defer r.Body.Close()

	fmt.Println("Hello client!")

	cName, err := saveFile(r, "content")

	if err != nil {
		fmt.Println("No content file!")
		w.WriteHeader(500)
		return
	}

	sName := r.FormValue("preStyle")

	if sName == "" {
		sName, err = saveFile(r, "style")

		if err != nil {
			fmt.Println("No style file!")
			w.WriteHeader(500)
			return
		}
	}

	gpu := r.FormValue("gpu");
	outName := RandStringBytes(6);

	w.Header().Set("Content-Type", "text/plain")
	io.WriteString(w, outName)

	go processImage(cName, sName, gpu, outName)

	fmt.Println("Goodbye client!")
}

func liveFeed(w http.ResponseWriter, r *http.Request) {

	name := r.FormValue("image")

	for !Exists("/home/jannek/serv2/output" + name + ".png") {
		time.Sleep(time.Second * 3)
	}


	w.Header().Set("Content-Type", "image/png")
	img, err := ioutil.ReadFile("/home/jannek/serv2/output" + name + ".png")

	if err != nil {
		fmt.Println("Image not found!")
		w.WriteHeader(500)
		return
	}

	_, err = w.Write(img)

	fmt.Println("Goodbye client!")
}

func main() {
	fmt.Println("Hello World!")
	
	rand.Seed(time.Now().UnixNano())

	http.HandleFunc("/", serve)
	http.HandleFunc("/live/", liveFeed)
	http.ListenAndServe(":8180", nil)
}
